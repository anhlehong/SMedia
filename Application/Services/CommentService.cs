using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CommentService(ICommentRepository commentRepository, IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<CommentDto> CreateCommentAsync(Guid userId, CommentCreateDto createDto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Người dùng không tồn tại");

        var post = await _commentRepository.GetPostAsync(createDto.PostId)
            ?? throw new KeyNotFoundException("Bài đăng không tồn tại");

        // Kiểm tra quyền truy cập nhóm nếu bài đăng thuộc nhóm
        if (post.GroupId.HasValue)
        {
            var isMember = await _commentRepository.IsGroupMemberAsync(userId, post.GroupId.Value);
            if (!isMember)
                throw new UnauthorizedAccessException("Bạn không phải thành viên nhóm");
        }

        // Kiểm tra comment cha nếu có
        if (createDto.ParentCommentId.HasValue)
        {
            var parentComment = await _commentRepository.GetCommentAsync(createDto.ParentCommentId.Value)
                ?? throw new KeyNotFoundException("Comment cha không tồn tại");
            if (parentComment.PostId != createDto.PostId)
                throw new InvalidOperationException("Comment cha không thuộc bài đăng này");
        }

        var comment = new Comment
        {
            CommentId = Guid.NewGuid(),
            PostId = createDto.PostId,
            UserId = userId,
            ParentCommentId = createDto.ParentCommentId,
            Content = createDto.Content,
            PostedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        var commentDto = comment.Adapt<CommentDto>();
        commentDto.Username = user.Username;
        return commentDto;
    }

    public async Task<CommentDto> UpdateCommentAsync(Guid userId, Guid commentId, CommentUpdateDto updateDto)
    {
        var comment = await _commentRepository.GetCommentAsync(commentId)
            ?? throw new KeyNotFoundException("Comment không tồn tại");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa comment này");

        comment.Content = updateDto.Content;
        await _commentRepository.UpdateAsync(comment);

        var user = await _userRepository.GetByIdAsync(comment.UserId)
            ?? throw new KeyNotFoundException("Người dùng không tồn tại");

        var commentDto = comment.Adapt<CommentDto>();
        commentDto.Username = user.Username;
        return commentDto;
    }

    public async Task<bool> DeleteCommentAsync(Guid userId, Guid commentId)
    {
        var comment = await _commentRepository.GetCommentAsync(commentId)
            ?? throw new KeyNotFoundException("Comment không tồn tại");

        // Kiểm tra quyền xóa: người tạo comment hoặc admin nhóm
        var post = await _commentRepository.GetPostAsync(comment.PostId)
            ?? throw new KeyNotFoundException("Bài đăng không tồn tại");

        bool isAuthorized = comment.UserId == userId;
        if (post.GroupId.HasValue && !isAuthorized)
        {
            isAuthorized = await _commentRepository.IsGroupAdminAsync(userId, post.GroupId.Value);
        }

        if (!isAuthorized)
            throw new UnauthorizedAccessException("Bạn không có quyền xóa comment này");

        return await _commentRepository.DeleteAsync(comment);
    }

    public async Task<List<CommentDto>> GetCommentsByPostAsync(Guid postId, int skip, int take, bool nested = false)
    {
        var post = await _commentRepository.GetPostAsync(postId)
            ?? throw new KeyNotFoundException("Bài đăng không tồn tại");

        List<Comment> comments;
        if (nested)
        {
            comments = await _commentRepository.GetNestedCommentsByPostAsync(postId, skip, take);
        }
        else
        {
            comments = await _commentRepository.GetFlatCommentsByPostAsync(postId, skip, take);
        }

        var commentDtos = new List<CommentDto>();
        foreach (var comment in comments)
        {
            var user = await _userRepository.GetByIdAsync(comment.UserId)
                ?? throw new KeyNotFoundException("Người dùng không tồn tại");
            var commentDto = comment.Adapt<CommentDto>();
            commentDto.Username = user.Username;
            commentDtos.Add(commentDto);
        }

        return commentDtos;
    }
}