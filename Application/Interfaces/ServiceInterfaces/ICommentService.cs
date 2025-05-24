using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(Guid userId, CommentCreateDto createDto);
    Task<CommentDto> UpdateCommentAsync(Guid userId, Guid commentId, CommentUpdateDto updateDto);
    Task<bool> DeleteCommentAsync(Guid userId, Guid commentId);
    Task<List<CommentDto>> GetCommentsByPostAsync(Guid postId, int skip, int take, bool nested = false);
}