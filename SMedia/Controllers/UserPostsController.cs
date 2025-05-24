using static Application.DTOs.PostDtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;

namespace SMedia.Controllers
{
    [Route("api/user-posts")]
    [ApiController]
    [Authorize]
    public class UserPostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public UserPostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> CreateUserPost([FromBody] PostCreateDto postDto)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            var post = await _postService.CreateUserPostAsync(postDto, userId);
            return CreatedAtAction(nameof(GetUserPosts), new { userId = post.UserId, page = 1, pageSize = 10 }, post);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<PostDto[]>> GetUserPosts(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var currentUserId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            var posts = await _postService.GetUserPostsAsync(userId, page, pageSize, currentUserId);
            return Ok(posts);
        }
    }
}