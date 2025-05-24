using Application.DTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsEmailExists(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsUsernameExists(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<User> Create(User newUser)
    {
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    // --------------------------------------------------------------------------------------------------------------------------------
    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    // public async Task<List<User>> SearchUsersAsync(string query, int skip, int take)
    // {
    //     var lowerQuery = query.ToLower();
    //     return await _context.Users
    //         .AsNoTracking()
    //         .Where(u => u.Username.ToLower().Contains(lowerQuery) ||
    //                     (u.FullName != null && u.FullName.ToLower().Contains(lowerQuery)) ||
    //                     (u.Email != null && u.Email.ToLower().Contains(lowerQuery)))
    //         .OrderBy(u => u.Username)
    //         .Skip(skip)
    //         .Take(take)
    //         .ToListAsync();
    // }
    //
    // public async Task<List<User>> GetRandomUsersAsync(Guid currentUserId, int count)
    // {
    //     return await _context.Users
    //         .AsNoTracking()
    //         .Where(u => u.UserId != currentUserId && u.Status == "Active")
    //         .OrderBy(_ => Guid.NewGuid())
    //         .Take(count)
    //         .ToListAsync();
    // }
    //
    // public async Task<List<User>> AdvancedSearchUsersAsync(AdvancedSearchDto searchDto, int skip, int take)
    // {
    //     var query = _context.Users.AsNoTracking().AsQueryable();
    //
    //     if (!string.IsNullOrEmpty(searchDto.Query))
    //     {
    //         var lowerQuery = searchDto.Query.ToLower();
    //         query = query.Where(u => u.Username.ToLower().Contains(lowerQuery) ||
    //                                  (u.FullName != null && u.FullName.ToLower().Contains(lowerQuery)) ||
    //                                  (u.Email != null && u.Email.ToLower().Contains(lowerQuery)));
    //     }
    //
    //     if (!string.IsNullOrEmpty(searchDto.Gender))
    //         query = query.Where(u => u.Gender == searchDto.Gender);
    //
    //     if (searchDto.JoinedAfter.HasValue)
    //         query = query.Where(u => u.JoinedAt >= searchDto.JoinedAfter);
    //
    //     if (!string.IsNullOrEmpty(searchDto.Status))
    //         query = query.Where(u => u.Status == searchDto.Status);
    //
    //     return await query
    //         .OrderBy(u => u.Username)
    //         .Skip(skip)
    //         .Take(take)
    //         .ToListAsync();
    // }
    public async Task<List<User>> SearchUsersAsync(string query, int page, int pageSize)
    {
        var lowerQuery = query.ToLower();
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Username.ToLower().Contains(lowerQuery) ||
                        (u.FullName != null && u.FullName.ToLower().Contains(lowerQuery)) ||
                        (u.Email != null && u.Email.ToLower().Contains(lowerQuery)))
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<User>> GetRandomUsersAsync(Guid currentUserId, int count)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.UserId != currentUserId && u.Status == "Active")
            .OrderBy(_ => Guid.NewGuid())
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<User>> AdvancedSearchUsersAsync(AdvancedSearchDto searchDto, int page, int pageSize)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.Query))
        {
            var lowerQuery = searchDto.Query.ToLower();
            query = query.Where(u => u.Username.ToLower().Contains(lowerQuery) ||
                                     (u.FullName != null && u.FullName.ToLower().Contains(lowerQuery)) ||
                                     (u.Email != null && u.Email.ToLower().Contains(lowerQuery)));
        }

        if (!string.IsNullOrEmpty(searchDto.Gender))
            query = query.Where(u => u.Gender == searchDto.Gender);

        if (searchDto.JoinedAfter.HasValue)
            query = query.Where(u => u.JoinedAt >= searchDto.JoinedAfter);

        if (!string.IsNullOrEmpty(searchDto.Status))
            query = query.Where(u => u.Status == searchDto.Status);

        return await query
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}