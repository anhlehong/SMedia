using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces;

public interface IUserRepository
{
    Task<bool> IsEmailExists(string email);
    Task<User> Create(User newUser);
    Task<bool> IsUsernameExists(string username);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(Guid userId);
    
    // -------------------------------------------------------------------------------------
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User?> GetByIdAsync(Guid userId);
    // Task<List<User>> SearchUsersAsync(string query, int skip, int take);
    // Task<List<User>> GetRandomUsersAsync(Guid currentUserId, int count);
    // Task<List<User>> AdvancedSearchUsersAsync(AdvancedSearchDto searchDto, int skip, int take);
    
    Task<List<User>> SearchUsersAsync(string query, int page, int pageSize);
    Task<List<User>> GetRandomUsersAsync(Guid currentUserId, int count);
    Task<List<User>> AdvancedSearchUsersAsync(AdvancedSearchDto searchDto, int page, int pageSize);
}