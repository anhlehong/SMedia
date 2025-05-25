using Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SMedia.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetMessageHistory([FromQuery] Guid? receiverId, [FromQuery] Guid? groupChatId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirst("user_id")?.Value);
        var messages = await _messageService.GetMessageHistoryAsync(userId, receiverId, groupChatId, page, pageSize);
        return Ok(messages);
    }
}