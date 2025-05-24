using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment> UpdateAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Comment?> GetCommentAsync(Guid commentId)
    {
        return await _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);
    }

    public async Task<Post?> GetPostAsync(Guid postId)
    {
        return await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<bool> IsGroupMemberAsync(Guid userId, Guid groupId)
    {
        return await _context.GroupMembers
            .AsNoTracking()
            .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId && gm.Status == "Active");
    }

    public async Task<bool> IsGroupAdminAsync(Guid userId, Guid groupId)
    {
        return await _context.GroupMembers
            .AsNoTracking()
            .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId && gm.Role == "Admin");
    }

    public async Task<List<Comment>> GetFlatCommentsByPostAsync(Guid postId, int skip, int take)
    {
        return await _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.PostedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetNestedCommentsByPostAsync(Guid postId, int skip, int take)
    {
        var topLevelComments = await _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.ChildComments)
            .Where(c => c.PostId == postId && c.ParentCommentId == null)
            .OrderByDescending(c => c.PostedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        // Tải comment con (giới hạn độ sâu để tối ưu)
        foreach (var comment in topLevelComments)
        {
            comment.ChildComments = await _context.Comments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.ParentCommentId == comment.CommentId)
                .OrderByDescending(c => c.PostedAt)
                .Take(10) // Giới hạn 10 comment con mỗi comment cha
                .ToListAsync();
        }

        return topLevelComments;
    }
}