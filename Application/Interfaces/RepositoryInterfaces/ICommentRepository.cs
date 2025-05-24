using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces;

public interface ICommentRepository
{
    Task<Comment> AddAsync(Comment comment);
    Task<Comment> UpdateAsync(Comment comment);
    Task<bool> DeleteAsync(Comment comment);
    Task<Comment?> GetCommentAsync(Guid commentId);
    Task<Post?> GetPostAsync(Guid postId);
    Task<bool> IsGroupMemberAsync(Guid userId, Guid groupId);
    Task<bool> IsGroupAdminAsync(Guid userId, Guid groupId);
    Task<List<Comment>> GetFlatCommentsByPostAsync(Guid postId, int skip, int take);
    Task<List<Comment>> GetNestedCommentsByPostAsync(Guid postId, int skip, int take);
}