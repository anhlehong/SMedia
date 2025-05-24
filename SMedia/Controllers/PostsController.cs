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
    [Route("api/posts")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{postId}")]
        public async Task<ActionResult<PostDto>> GetPostById(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            var post = await _postService.GetPostByIdAsync(postId, userId);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        [HttpPut("{postId}")]
        public async Task<ActionResult<PostDto>> UpdatePost(Guid postId, [FromBody] PostUpdateDto postDto)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            var post = await _postService.UpdatePostAsync(postId, postDto, userId);
            return Ok(post);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            await _postService.DeletePostAsync(postId, userId);
            return NoContent();
        }

        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<StaticCommentDto>> CreateComment(Guid postId, [FromBody] StaticCommentCreateDto commentDto)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            var comment = await _postService.CreateCommentAsync(postId, commentDto, userId);
            return CreatedAtAction(nameof(GetPostById), new { postId }, comment);
        }

        [HttpPost("{postId}/vote")]
        public async Task<IActionResult> ToggleVotePost(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException("Invalid token."));
            await _postService.ToggleVotePostAsync(postId, userId);
            return Ok();
        }
    }
}