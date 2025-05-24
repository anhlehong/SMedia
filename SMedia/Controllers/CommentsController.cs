using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SMedia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromQuery] Guid userId, [FromBody] CommentCreateDto createDto)
    {
        var comment = await _commentService.CreateCommentAsync(userId, createDto);
        return CreatedAtAction(nameof(GetCommentsByPost), new { postId = comment.PostId }, comment);
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromQuery] Guid userId, [FromBody] CommentUpdateDto updateDto)
    {
        var comment = await _commentService.UpdateCommentAsync(userId, commentId, updateDto);
        return Ok(comment);
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId, [FromQuery] Guid userId)
    {
        await _commentService.DeleteCommentAsync(userId, commentId);
        return NoContent();
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetCommentsByPost(Guid postId, 
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 10, 
        [FromQuery] bool nested = false)
    {
        var comments = await _commentService.GetCommentsByPostAsync(postId, skip, take, nested);
        return Ok(comments);
    }
}