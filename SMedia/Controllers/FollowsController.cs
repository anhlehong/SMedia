using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FollowsController : ControllerBase
{
    private readonly IFollowService _followService;

    public FollowsController(IFollowService followService)
    {
        _followService = followService;
    }

    [HttpPost]
    public async Task<IActionResult> FollowUser([FromQuery] Guid followerId, [FromQuery] Guid followedId)
    {
        var follow = await _followService.FollowUserAsync(followerId, followedId);
        return CreatedAtAction(nameof(IsFollowing), new { followerId, followedId }, follow);
    }

    [HttpDelete]
    public async Task<IActionResult> UnfollowUser([FromQuery] Guid followerId, [FromQuery] Guid followedId)
    {
        await _followService.UnfollowUserAsync(followerId, followedId);
        return NoContent();
    }

    [HttpGet("followers/{userId}")]
    public async Task<IActionResult> GetFollowers(Guid userId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var followers = await _followService.GetFollowersAsync(userId, skip, take);
        return Ok(followers);
    }

    [HttpGet("following/{userId}")]
    public async Task<IActionResult> GetFollowing(Guid userId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var following = await _followService.GetFollowingAsync(userId, skip, take);
        return Ok(following);
    }

    [HttpGet("followers/count/{userId}")]
    public async Task<IActionResult> GetFollowersCount(Guid userId)
    {
        var count = await _followService.GetFollowersCountAsync(userId);
        return Ok(count);
    }

    [HttpGet("following/count/{userId}")]
    public async Task<IActionResult> GetFollowingCount(Guid userId)
    {
        var count = await _followService.GetFollowingCountAsync(userId);
        return Ok(count);
    }

    [HttpGet("is-following")]
    public async Task<IActionResult> IsFollowing([FromQuery] Guid followerId, [FromQuery] Guid followedId)
    {
        var isFollowing = await _followService.IsFollowingAsync(followerId, followedId);
        return Ok(isFollowing);
    }

    [HttpGet("suggestions/{userId}")]
    public async Task<IActionResult> GetFollowSuggestions(Guid userId, [FromQuery] int count = 5)
    {
        var suggestions = await _followService.GetFollowSuggestionsAsync(userId, count);
        return Ok(suggestions);
    }
}