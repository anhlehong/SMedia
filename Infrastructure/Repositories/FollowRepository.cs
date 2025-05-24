using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly ApplicationDbContext _context;

    public FollowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserFollow?> GetFollowAsync(Guid followerId, Guid followedId)
    {
        return await _context.UserFollows
            .AsNoTracking()
            .Include(f => f.Follower)
            .Include(f => f.Followed)
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public async Task<UserFollow> AddFollowAsync(UserFollow follow)
    {
        _context.UserFollows.Add(follow);
        await _context.SaveChangesAsync();
        return follow;
    }

    public async Task<bool> RemoveFollowAsync(UserFollow follow)
    {
        _context.UserFollows.Remove(follow);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<UserFollow>> GetFollowersAsync(Guid userId, int skip, int take)
    {
        return await _context.UserFollows
            .AsNoTracking()
            .Include(f => f.Follower)
            .Include(f => f.Followed)
            .Where(f => f.FollowedId == userId)
            .OrderByDescending(f => f.FollowedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<UserFollow>> GetFollowingAsync(Guid userId, int skip, int take)
    {
        return await _context.UserFollows
            .AsNoTracking()
            .Include(f => f.Follower)
            .Include(f => f.Followed)
            .Where(f => f.FollowerId == userId)
            .OrderByDescending(f => f.FollowedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetFollowersCountAsync(Guid userId)
    {
        return await _context.UserFollows
            .AsNoTracking()
            .CountAsync(f => f.FollowedId == userId);
    }

    public async Task<int> GetFollowingCountAsync(Guid userId)
    {
        return await _context.UserFollows
            .AsNoTracking()
            .CountAsync(f => f.FollowerId == userId);
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followedId)
    {
        return await _context.UserFollows
            .AsNoTracking()
            .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public async Task<List<(Guid UserId, string Username, string? FullName, string? Image, int MutualFollowersCount)>> GetFollowSuggestionsAsync(Guid userId, int count)
    {
        var suggestions = await _context.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowerId != userId &&
                        _context.UserFollows.Any(ff => ff.FollowerId == userId && ff.FollowedId == f.FollowerId) &&
                        !_context.UserFollows.Any(ff => ff.FollowerId == userId && ff.FollowedId == f.FollowedId))
            .GroupBy(f => f.FollowedId)
            .Select(g => new
            {
                UserId = g.Key,
                MutualFollowersCount = g.Count()
            })
            .Join(_context.Users,
                g => g.UserId,
                u => u.UserId,
                (g, u) => new
                {
                    u.UserId,
                    u.Username,
                    u.FullName,
                    u.Image,
                    g.MutualFollowersCount
                })
            .OrderByDescending(x => x.MutualFollowersCount)
            .Take(count)
            .Select(x => new
            {
                x.UserId,
                x.Username,
                x.FullName,
                x.Image,
                x.MutualFollowersCount
            })
            .ToListAsync();

        return suggestions.Select(x => (x.UserId, x.Username, x.FullName, x.Image, x.MutualFollowersCount)).ToList();
    }
}