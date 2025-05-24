using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;

namespace SMedia.Controllers;

[Route("api/group-members")]
[ApiController]
[Authorize]
public class GroupMembersController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupMembersController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost("request")]
    public async Task<ActionResult<GroupMemberDto>> RequestJoinGroup([FromBody] GroupMemberRequestDto requestDto)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var userId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            var member = await _groupService.RequestJoinGroupAsync(requestDto, userId);
            return CreatedAtAction(nameof(RequestJoinGroup), new { groupId = member.GroupId, userId = member.UserId },
                member);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error requesting join group {requestDto.GroupId}: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while requesting to join the group." });
        }
    }

    [HttpPost("{groupId}/approve")]
    public async Task<ActionResult<GroupMemberDto>> ApproveMember(Guid groupId,
        [FromBody] GroupMemberApproveDto approveDto)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var adminId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            var member = await _groupService.ApproveMemberAsync(groupId, approveDto, adminId);
            return Ok(member);
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
            Console.WriteLine($"Error approving member {approveDto.UserId} for group {groupId}: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while approving the member." });
        }
    }

    [HttpDelete("{groupId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var adminId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            await _groupService.RemoveMemberAsync(groupId, userId, adminId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing member {userId} from group {groupId}: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while removing the member." });
        }
    }
    [HttpGet("{groupId}/is-member-group")]
    public async Task<IActionResult> IsMemberOfGroup(Guid groupId)
    {
        try
        {
            if(!Guid.TryParse(User.FindFirst("user_id")?.Value, out var userId))
                return Unauthorized( new { Error = "Invalid token: user_id is missing or invalid." });
            
            var isMember = await _groupService.IsMemberOfGroupAsync(groupId, userId);
            return Ok(isMember);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpGet("{groupId}/members")]
    public async Task<IActionResult> GetGroupMembers(Guid groupId)
    {
        var members = await _groupService.GetGroupMemberAsync(groupId);
        return Ok(members);
    }
}