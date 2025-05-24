using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces;

public interface IFollowRepository
{
    Task<UserFollow?> GetFollowAsync(Guid followerId, Guid followedId);
    Task<UserFollow> AddFollowAsync(UserFollow follow);
    Task<bool> RemoveFollowAsync(UserFollow follow);
    Task<List<UserFollow>> GetFollowersAsync(Guid userId, int skip, int take);
    Task<List<UserFollow>> GetFollowingAsync(Guid userId, int skip, int take);
    Task<int> GetFollowersCountAsync(Guid userId);
    Task<int> GetFollowingCountAsync(Guid userId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followedId);
    Task<List<(Guid UserId, string Username, string? FullName, string? Image, int MutualFollowersCount)>> GetFollowSuggestionsAsync(Guid userId, int count);
}