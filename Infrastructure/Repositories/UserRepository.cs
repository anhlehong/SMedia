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
        return await _context.users.AnyAsync( u => u.email == email);
    }
    public async Task<bool> IsUsernameExists(string username)
    {
        return await _context.users.AnyAsync( u => u.username == username);
    }

    public async Task<user> Create(user newUser)
    {
        _context.users.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
    
    public async Task<user?> GetUserByEmail(string email)
    {
        return await _context.users.FirstOrDefaultAsync(u => u.email == email);
    }
}
