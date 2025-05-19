using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IMessageService _messageService;

    public HomeController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
    {
        var currentUserId = User.FindFirst("user_id")?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var users = await _messageService.SearchUsersAsync(searchTerm, currentUserId);
        return Ok(users);
    }
}