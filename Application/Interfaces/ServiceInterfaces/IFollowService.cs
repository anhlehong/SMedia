using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface IFollowService
{
    Task<FollowDto> FollowUserAsync(Guid followerId, Guid followedId);
    Task<bool> UnfollowUserAsync(Guid followerId, Guid followedId);
    Task<List<FollowDto>> GetFollowersAsync(Guid userId, int skip, int take);
    Task<List<FollowDto>> GetFollowingAsync(Guid userId, int skip, int take);
    Task<int> GetFollowersCountAsync(Guid userId);
    Task<int> GetFollowingCountAsync(Guid userId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followedId);
    Task<List<UserSuggestionDto>> GetFollowSuggestionsAsync(Guid userId, int count);
}