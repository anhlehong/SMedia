using static Application.DTOs.PostDtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.DTOs;

namespace SMedia.Controllers
{
    [Route("api/group-posts")]
    [ApiController]
    [Authorize]
    public class GroupPostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public GroupPostsController(IPostService postService, IUserService userService)
        {
            _postService = postService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> CreateGroupPost([FromBody] GroupPostCreateDto postDto)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            var post = await _postService.CreateGroupPostAsync(postDto, userId);
            return CreatedAtAction(nameof(GetGroupPosts), new { groupId = post.GroupId, page = 1, pageSize = 10 }, post);
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<PostImageDto[]>> GetGroupPosts(Guid groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var currentUserId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            var posts = await _postService.GetGroupPostsAsync(groupId, page, pageSize, currentUserId);
            return Ok(posts);
        }
        
        [HttpPost("{groupId}/approve")]
        public async Task<ActionResult<PostDto>> ApproveGroupPost(Guid groupId, [FromBody] GroupPostApproveDto approveDto)
        {
            try
            {
                if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var adminId))
                    return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

                var post = await _postService.ApproveGroupPostAsync(groupId, approveDto, adminId);
                return Ok(post);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving post {approveDto.PostId} for group {groupId}: {ex.Message}");
                return StatusCode(500, new { Error = "An error occurred while approving the post." });
            }
        }        [HttpGet("pending/{groupId}")]
        public async Task<ActionResult<PendingPostDto[]>> GetPendingGroupPosts(Guid groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var currentUserId))
                    return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

                var posts = await _postService.GetPendingGroupPostsAsync(groupId, page, pageSize, currentUserId);
                
                // Transform the PostDto to PendingPostDto format
                List<PendingPostDto> pendingPosts = new List<PendingPostDto>();
                  foreach (var p in posts)
                {
                    pendingPosts.Add(new PendingPostDto
                    {
                        Id = p.PostId,
                        User = new PostUserDto
                        {
                            Id = p.UserId,
                            Name = await GetUserNameById(p.UserId),
                            AvatarUrl = await GetUserAvatarById(p.UserId)
                        },
                        Content = p.Content,
                        Media = p.Media.Select(m => new PendingPostMediaDto
                        {
                            Id = m.MediaId,
                            Url = m.MediaUrl,
                            Type = m.MediaType
                        }).ToList(),
                        CreatedAt = p.PostedAt,
                        GroupId = p.GroupId ?? groupId,
                        IsVisible = p.IsVisible
                    });
                }

                return Ok(pendingPosts);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving pending posts for group {groupId}: {ex.Message}");
                return StatusCode(500, new { Error = "An error occurred while retrieving pending group posts." });
            }
        }        [HttpPut("visible")]
        public async Task<ActionResult<PostDto>> UpdateGroupPostVisibility([FromBody] GroupPostVisibilityDto visibilityDto)
        {
            try
            {
                if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var adminId))
                    return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

                // First, get the post to determine which group it belongs to
                var currentPost = await _postService.GetPostByIdAsync(visibilityDto.PostId, adminId);
                if (currentPost == null)
                    return NotFound(new { Error = "Post not found." });

                if (!currentPost.GroupId.HasValue)
                    return BadRequest(new { Error = "This operation is only applicable to group posts." });

                var post = await _postService.UpdateGroupPostVisibilityAsync(currentPost.GroupId.Value, visibilityDto, adminId);
                return Ok(post);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating visibility for post {visibilityDto.PostId}: {ex.Message}");
                return StatusCode(500, new { Error = "An error occurred while updating the post visibility." });
            }
        }

        // Helper methods to get user information
        private async Task<string> GetUserNameById(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return user.FullName ?? user.Username;
            }
            catch (Exception)
            {
                // If user not found, return a default name
                return "Unknown User";
            }
        }

        private async Task<string> GetUserAvatarById(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return user.Image ?? "/avatar.png";
            }
            catch (Exception)
            {
                // If user not found or has no avatar, return a default avatar
                return "/avatar.png";
            }
        }
    }
}