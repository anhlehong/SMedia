using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces;

public interface IPostRepository
{
    Task<Post> CreatePostAsync(Post post);
    Task<Post?> GetPostByIdAsync(Guid postId);
    Task<List<Post>> GetUserPostsAsync(Guid userId, int page, int pageSize);
    Task<List<Post>> GetGroupPostsAsync(Guid groupId, int page, int pageSize);
    Task<List<Post>> GetPendingGroupPostsAsync(Guid groupId, int page, int pageSize);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(Guid postId);
    Task CreateCommentAsync(Comment comment);
    Task CreateVoteAsync(PostVote vote);
    Task<PostVote?> GetVoteAsync(Guid userId, Guid postId);
    Task DeleteVoteAsync(Guid userId, Guid postId);
    Task CreateMediaAsync(Media media);
    Task DeleteMediaByPostIdAsync(Guid postId);
    Task<bool> IsGroupMemberAsync(Guid userId, Guid groupId);
    Task<bool> IsGroupAdminAsync(Guid userId, Guid groupId);
    Task SetPostsInvisibleByUserInGroupAsync(Guid userId, Guid groupId);
    Task<List<Post>> GetGroupPostsImageAsync(Guid groupId, int page, int pageSize);
}