using static Application.DTOs.PostDtos;
using Mapster;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<PostDto> CreateUserPostAsync(PostCreateDto postDto, Guid userId)
        {
            try
            {
                var post = (postDto, userId).Adapt<Post>();

                await _postRepository.CreatePostAsync(post);

                foreach (var mediaDto in postDto.Media)
                {
                    var media = mediaDto.Adapt<Media>();
                    media.UploadedBy = userId;
                    media.PostId = post.PostId;
                    await _postRepository.CreateMediaAsync(media);
                }

                var resultDto = post.Adapt<PostDto>();
                resultDto.IsVotedByCurrentUser = false;
                Console.WriteLine($"Created user post {post.PostId} for user {userId}");
                return resultDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user post for user {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<PostDto> CreateGroupPostAsync(GroupPostCreateDto postDto, Guid userId)
        {
            try
            {
                var isMember = await _postRepository.IsGroupMemberAsync(userId, postDto.GroupId);
                if (!isMember)
                    throw new UnauthorizedAccessException("User is not a member of the group.");

                var post = (postDto, userId).Adapt<Post>();

                await _postRepository.CreatePostAsync(post);

                foreach (var mediaDto in postDto.Media)
                {
                    var media = mediaDto.Adapt<Media>();
                    media.UploadedBy = userId;
                    media.PostId = post.PostId;
                    await _postRepository.CreateMediaAsync(media);
                }

                var resultDto = post.Adapt<PostDto>();
                resultDto.IsVotedByCurrentUser = false;
                Console.WriteLine($"Created group post {post.PostId} for group {postDto.GroupId}");
                return resultDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating group post for user {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid postId, Guid currentUserId)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                {
                    Console.WriteLine($"Post {postId} not found");
                    return null;
                }
                if (post.GroupId.HasValue)
                {
                    var isMember = await _postRepository.IsGroupMemberAsync(currentUserId, post.GroupId.Value);
                    if (!isMember)
                        throw new UnauthorizedAccessException("User is not a member of the group.");
                }

                var postDto = post.Adapt<PostDto>();
                postDto.IsVotedByCurrentUser = post.PostVotes.Any(v => v.UserId == currentUserId && v.VoteType == "Vote");
                Console.WriteLine($"Retrieved post {postId} for user {currentUserId}");
                return postDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting post {postId}: {ex.Message}");
                throw;
            }
        }

        public async Task<PostDto[]> GetUserPostsAsync(Guid userId, int page, int pageSize, Guid currentUserId)
        {
            try
            {
                var posts = await _postRepository.GetUserPostsAsync(userId, page, pageSize);
                var postDtos = posts.Select(p =>
                {
                    var dto = p.Adapt<PostDto>();
                    dto.IsVotedByCurrentUser = p.PostVotes.Any(v => v.UserId == currentUserId && v.VoteType == "Vote");
                    return dto;
                }).ToArray();
                Console.WriteLine($"Retrieved {postDtos.Length} user posts for user {userId}, page {page}");
                return postDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user posts for user {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<PostImageDto[]> GetGroupPostsAsync(Guid groupId, int page, int pageSize, Guid currentUserId)
        {
            try
            {
                var isMember = await _postRepository.IsGroupMemberAsync(currentUserId, groupId);
                if (!isMember)
                    throw new UnauthorizedAccessException("User is not a member of the group.");

                var posts = await _postRepository.GetGroupPostsImageAsync(groupId, page, pageSize);
                var postDtos = posts.Select(p =>
                {
                    var dto = p.Adapt<PostImageDto>();
                    dto.userName = p.User.Username; // Đổi từ Username
                    dto.userAvatar = p.User.Image; // Đổi từ UserAvatar
                    dto.IsVotedByCurrentUser = p.PostVotes.Any(v => v.UserId == currentUserId && v.VoteType == "Vote");
                    dto.VoteCount = p.PostVotes.Count(v => v.VoteType == "Vote");
                    dto.CommentCount = p.Comments.Count;
                    return dto;
                }).ToArray();
            
                Console.WriteLine($"Retrieved {postDtos.Length} group posts for group {groupId}, page {page}");
                return postDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting group posts for group {groupId}: {ex.Message}");
                throw;
            }
        }

        public async Task<PostDto> UpdatePostAsync(Guid postId, PostUpdateDto postDto, Guid userId)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                    throw new KeyNotFoundException("Post not found.");
                if (post.UserId != userId)
                {
                    if (post.GroupId.HasValue)
                    {
                        var isAdmin = await _postRepository.IsGroupAdminAsync(userId, post.GroupId.Value);
                        if (!isAdmin)
                            throw new UnauthorizedAccessException("You are not authorized to update this post.");
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("You are not authorized to update this post.");
                    }
                }

                postDto.Adapt(post);
                await _postRepository.DeleteMediaByPostIdAsync(postId);

                foreach (var mediaDto in postDto.Media)
                {
                    var media = mediaDto.Adapt<Media>();
                    media.UploadedBy = userId;
                    media.PostId = postId;
                    await _postRepository.CreateMediaAsync(media);
                }

                await _postRepository.UpdatePostAsync(post);
                var updatedPost = await _postRepository.GetPostByIdAsync(postId);
                if (updatedPost == null)
                    throw new InvalidOperationException("Failed to retrieve updated post.");

                var resultDto = updatedPost.Adapt<PostDto>();
                resultDto.IsVotedByCurrentUser = updatedPost.PostVotes.Any(v => v.UserId == userId && v.VoteType == "Vote");
                Console.WriteLine($"Updated post {postId} for user {userId}");
                return resultDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating post {postId}: {ex.Message}");
                throw;
            }
        }
        
        public async Task<PostDto> ApproveGroupPostAsync(Guid groupId, GroupPostApproveDto approveDto, Guid adminId)
        {
            try
            {
                var isAdmin = await _postRepository.IsGroupAdminAsync(adminId, groupId);
                if (!isAdmin)
                    throw new UnauthorizedAccessException("User is not an admin of the group.");

                var post = await _postRepository.GetPostByIdAsync(approveDto.PostId);
                if (post == null || post.GroupId != groupId)
                    throw new KeyNotFoundException("Post not found in the group.");

                if (post.IsApproved == approveDto.Approve)
                    throw new InvalidOperationException($"Post is already {(approveDto.Approve ? "approved" : "rejected")}.");

                post.IsApproved = approveDto.Approve;
                post.IsVisible = approveDto.Approve; // Nếu rejected, set IsVisible = false
                await _postRepository.UpdatePostAsync(post);

                var postDto = post.Adapt<PostDto>();
                postDto.IsVotedByCurrentUser = post.PostVotes.Any(v => v.UserId == adminId && v.VoteType == "Vote");
                Console.WriteLine($"Approved post {approveDto.PostId} in group {groupId}, status: {post.IsApproved}");
                return postDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving post {approveDto.PostId} in group {groupId}: {ex.Message}");
                throw;
            }
        }

        public async Task DeletePostAsync(Guid postId, Guid userId)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                    throw new KeyNotFoundException("Post not found.");
                if (post.UserId != userId)
                {
                    if (post.GroupId.HasValue)
                    {
                        var isAdmin = await _postRepository.IsGroupAdminAsync(userId, post.GroupId.Value);
                        if (!isAdmin)
                            throw new UnauthorizedAccessException("You are not authorized to delete this post.");
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("You are not authorized to delete this post.");
                    }
                }

                await _postRepository.DeletePostAsync(postId);
                Console.WriteLine($"Deleted post {postId} for user {userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post {postId}: {ex.Message}");
                throw;
            }
        }

        public async Task<StaticCommentDto> CreateCommentAsync(Guid postId, StaticCommentCreateDto commentDto, Guid userId)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                    throw new KeyNotFoundException("Post not found.");
                if (post.GroupId.HasValue)
                {
                    var isMember = await _postRepository.IsGroupMemberAsync(userId, post.GroupId.Value);
                    if (!isMember)
                        throw new UnauthorizedAccessException("User is not a member of the group.");
                }

                var comment = commentDto.Adapt<Comment>();
                comment.CommentId = Guid.NewGuid();
                comment.PostId = postId;
                comment.UserId = userId;
                comment.PostedAt = DateTime.UtcNow;

                await _postRepository.CreateCommentAsync(comment);
                Console.WriteLine($"Created comment {comment.CommentId} for post {postId}");
                return comment.Adapt<StaticCommentDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating comment for post {postId}: {ex.Message}");
                throw;
            }
        }

        public async Task ToggleVotePostAsync(Guid postId, Guid userId)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                    throw new KeyNotFoundException("Post not found.");
                if (post.GroupId.HasValue)
                {
                    var isMember = await _postRepository.IsGroupMemberAsync(userId, post.GroupId.Value);
                    if (!isMember)
                        throw new UnauthorizedAccessException("User is not a member of the group.");
                }

                var existingVote = await _postRepository.GetVoteAsync(userId, postId);
                if (existingVote != null)
                {
                    await _postRepository.DeleteVoteAsync(userId, postId);
                    Console.WriteLine($"Deleted vote for post {postId} by user {userId}");
                    return;
                }

                var vote = new PostVote
                {
                    UserId = userId,
                    PostId = postId,
                    VoteType = "Vote",
                    VotedAt = DateTime.UtcNow
                };

                await _postRepository.CreateVoteAsync(vote);
                Console.WriteLine($"Created vote for post {postId} by user {userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error voting on post {postId}: {ex.Message}");
                throw;
            }
        }        public async Task<PostDto[]> GetPendingGroupPostsAsync(Guid groupId, int page, int pageSize, Guid currentUserId)
        {
            try
            {
                var isAdmin = await _postRepository.IsGroupAdminAsync(currentUserId, groupId);
                if (!isAdmin)
                    throw new UnauthorizedAccessException("Only group administrators can access pending posts.");

                var posts = await _postRepository.GetPendingGroupPostsAsync(groupId, page, pageSize);

                var postsDto = posts.Select(p =>
                {
                    var dto = p.Adapt<PostDto>();
                    dto.IsVotedByCurrentUser = p.PostVotes.Any(v => v.UserId == currentUserId && v.VoteType == "Vote");
                    return dto;
                }).ToArray();

                Console.WriteLine($"Retrieved {postsDto.Length} pending visible posts for group {groupId}");
                return postsDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pending visible posts for group {groupId}: {ex.Message}");
                throw;
            }
        }

        public async Task<PostDto> UpdateGroupPostVisibilityAsync(Guid groupId, GroupPostVisibilityDto visibilityDto, Guid adminId)
        {
            try
            {
                var isAdmin = await _postRepository.IsGroupAdminAsync(adminId, groupId);
                if (!isAdmin)
                    throw new UnauthorizedAccessException("User is not an admin of the group.");

                var post = await _postRepository.GetPostByIdAsync(visibilityDto.PostId);
                if (post == null || post.GroupId != groupId)
                    throw new KeyNotFoundException("Post not found in the group.");

                if (post.IsVisible == visibilityDto.IsVisible)
                    throw new InvalidOperationException($"Post visibility is already set to {(visibilityDto.IsVisible ? "visible" : "hidden")}.");

                post.IsVisible = visibilityDto.IsVisible;
                await _postRepository.UpdatePostAsync(post);

                var postDto = post.Adapt<PostDto>();
                postDto.IsVotedByCurrentUser = post.PostVotes.Any(v => v.UserId == adminId && v.VoteType == "Vote");
                Console.WriteLine($"Updated visibility for post {visibilityDto.PostId} in group {groupId}, visibility: {post.IsVisible}");
                return postDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating visibility for post {visibilityDto.PostId} in group {groupId}: {ex.Message}");
                throw;
            }
        }
    }
}