using Application.DTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Mapster;

namespace Application.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;

    public FollowService(IFollowRepository followRepository)
    {
        _followRepository = followRepository;
    }

    public async Task<FollowDto> FollowUserAsync(Guid followerId, Guid followedId)
    {
        if (followerId == followedId)
            throw new InvalidOperationException("Không thể theo dõi chính mình");

        var existingFollow = await _followRepository.GetFollowAsync(followerId, followedId);
        if (existingFollow != null)
            throw new InvalidOperationException("Đã theo dõi người dùng này");

        var follow = new UserFollow()
        {
            FollowerId = followerId,
            FollowedId = followedId,
            FollowedAt = DateTime.UtcNow
        };

        await _followRepository.AddFollowAsync(follow);
        return follow.Adapt<FollowDto>();
    }

    public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followedId)
    {
        var follow = await _followRepository.GetFollowAsync(followerId, followedId)
            ?? throw new KeyNotFoundException("Mối quan hệ theo dõi không tồn tại");

        return await _followRepository.RemoveFollowAsync(follow);
    }

    public async Task<List<FollowDto>> GetFollowersAsync(Guid userId, int skip, int take)
    {
        var followers = await _followRepository.GetFollowersAsync(userId, skip, take);
        return followers.Adapt<List<FollowDto>>();
    }

    public async Task<List<FollowDto>> GetFollowingAsync(Guid userId, int skip, int take)
    {
        var following = await _followRepository.GetFollowingAsync(userId, skip, take);
        return following.Adapt<List<FollowDto>>();
    }

    public async Task<int> GetFollowersCountAsync(Guid userId)
    {
        return await _followRepository.GetFollowersCountAsync(userId);
    }

    public async Task<int> GetFollowingCountAsync(Guid userId)
    {
        return await _followRepository.GetFollowingCountAsync(userId);
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followedId)
    {
        return await _followRepository.IsFollowingAsync(followerId, followedId);
    }
    public async Task<List<UserSuggestionDto>> GetFollowSuggestionsAsync(Guid userId, int count)
    {
        var suggestions = await _followRepository.GetFollowSuggestionsAsync(userId, count);
        return suggestions.Adapt<List<UserSuggestionDto>>();
    }
}