using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                Console.WriteLine($"Created post {post.PostId} for user {post.UserId}");
                return post;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating post {post.PostId}: {ex.Message}");
                throw;
            }
        }

        public async Task<Post?> GetPostByIdAsync(Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.Media)
                .Include(p => p.Comments)
                .Include(p => p.PostVotes)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                Console.WriteLine($"Post {postId} not found");
            }

            return post;
        }

        public async Task<List<Post>> GetUserPostsAsync(Guid userId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.Media)
                .Include(p => p.Comments)
                .Include(p => p.PostVotes)
                .Where(p => p.UserId == userId && p.GroupId == null && p.IsVisible == true)
                .OrderByDescending(p => p.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine(
                $"Retrieved {posts.Count} user posts for user {userId}, page {page}, pageSize {pageSize}");
            return posts;
        }

        public async Task<List<Post>> GetGroupPostsAsync(Guid groupId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.Media)
                .Include(p => p.Comments)
                .Include(p => p.PostVotes)
                .Where(p => p.GroupId == groupId && p.IsApproved == true && p.IsVisible == true)
                .OrderByDescending(p => p.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine(
                $"Retrieved {posts.Count} group posts for group {groupId}, page {page}, pageSize {pageSize}");
            return posts;
        }

        public async Task<List<Post>> GetGroupPostsImageAsync(Guid groupId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Media)
                .Include(p => p.Comments)
                .Include(p => p.PostVotes)
                .Where(p => p.GroupId == groupId && p.IsApproved == true && p.IsVisible == true)
                .OrderByDescending(p => p.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Retrieved {posts.Count} group posts for group {groupId}, page {page}, pageSize {pageSize}");
            return posts;
        }
        
        public async Task<List<Post>> GetPendingGroupPostsAsync(Guid groupId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.Media)
                .Include(p => p.Comments)
                .Include(p => p.PostVotes)
                .Where(p => p.GroupId == groupId && p.IsApproved == false && p.IsVisible == true)
                .OrderByDescending(p => p.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine(
                $"Retrieved {posts.Count} pending visible posts for group {groupId}, page {page}, pageSize {pageSize}");
            return posts;
        }

        public async Task UpdatePostAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Updated post {post.PostId}");
        }

        public async Task DeletePostAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.IsVisible = false;
                await _context.SaveChangesAsync();
                Console.WriteLine($"Deleted post {postId}");
            }
            else
            {
                Console.WriteLine($"Post {postId} not found for deletion");
            }
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Created comment {comment.CommentId} for post {comment.PostId}");
        }

        public async Task CreateVoteAsync(PostVote vote)
        {
            _context.PostVotes.Add(vote);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Created vote for post {vote.PostId} by user {vote.UserId}");
        }

        public async Task<PostVote?> GetVoteAsync(Guid userId, Guid postId)
        {
            var vote = await _context.PostVotes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.PostId == postId && v.VoteType == "Vote");
            if (vote == null)
            {
                Console.WriteLine($"No vote found for post {postId} by user {userId}");
            }

            return vote;
        }

        public async Task DeleteVoteAsync(Guid userId, Guid postId)
        {
            var vote = await _context.PostVotes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.PostId == postId && v.VoteType == "Vote");
            if (vote != null)
            {
                _context.PostVotes.Remove(vote);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Deleted vote for post {postId} by user {userId}");
            }
        }

        public async Task CreateMediaAsync(Media media)
        {
            _context.Media.Add(media);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Created media {media.MediaId} for post {media.PostId}");
        }

        public async Task DeleteMediaByPostIdAsync(Guid postId)
        {
            var media = await _context.Media
                .Where(m => m.PostId == postId)
                .ToListAsync();
            _context.Media.RemoveRange(media);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Deleted {media.Count} media items for post {postId}");
        }

        public async Task<bool> IsGroupMemberAsync(Guid userId, Guid groupId)
        {
            var isMember = await _context.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.Status == "Active");
            Console.WriteLine($"User {userId} is {(isMember ? "" : "not")} a member of group {groupId}");
            return isMember;
        }

        public async Task<bool> IsGroupAdminAsync(Guid userId, Guid groupId)
        {
            var isAdmin = await _context.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.Role == "Admin" && m.Status == "Active");
            Console.WriteLine($"User {userId} is {(isAdmin ? "" : "not")} an admin of group {groupId}");
            return isAdmin;
        }

        public async Task SetPostsInvisibleByUserInGroupAsync(Guid userId, Guid groupId)
        {
            var posts = await _context.Posts
                .Where(p => p.UserId == userId && p.GroupId == groupId && p.IsVisible == true)
                .ToListAsync();

            foreach (var post in posts)
            {
                post.IsVisible = false;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Set {posts.Count} posts invisible for user {userId} in group {groupId}");
        }
    }
}