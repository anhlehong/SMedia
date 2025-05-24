using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;

namespace SMedia.Controllers;

[Route("api/groups")]
[ApiController]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] GroupCreateDto groupDto)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var userId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            var group = await _groupService.CreateGroupAsync(groupDto, userId);
            return CreatedAtAction(nameof(GetGroupById), new { groupId = group.GroupId }, group);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating group: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while creating the group." });
        }
    }

    [HttpGet("{groupId}")]
    public async Task<ActionResult<GroupDto>> GetGroupById(Guid groupId)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var userId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            var group = await _groupService.GetGroupByIdAsync(groupId, userId);
            return Ok(group);
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
            Console.WriteLine($"Error getting group {groupId}: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while retrieving the group." });
        }
    }

    [HttpGet]
    public async Task<ActionResult<GroupDto[]>> GetGroups([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var userId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            var groups = await _groupService.GetGroupsAsync(page, pageSize, userId);
            return Ok(groups);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting groups: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while retrieving groups." });
        }
    }

    [HttpPut("{groupId}")]
    public async Task<ActionResult<GroupDto>> UpdateGroup(Guid groupId, [FromBody] GroupUpdateDto groupDto)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var userId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            var group = await _groupService.UpdateGroupAsync(groupId, groupDto, userId);
            return Ok(group);
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
            Console.WriteLine($"Error updating group {groupId}: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while updating the group." });
        }
    }

    [HttpDelete("{groupId}")]
    public async Task<IActionResult> DeleteGroup(Guid groupId)
    {
        try
        {
            if (!Guid.TryParse(User.FindFirst("user_id")?.Value, out var userId))
                return Unauthorized(new { Error = "Invalid token: user_id is missing or invalid." });

            await _groupService.DeleteGroupAsync(groupId, userId);
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
            Console.WriteLine($"Error deleting group {groupId}: {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while deleting the group." });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<GroupDto[]>> SearchGroups([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new { Error = "Search term cannot be empty." });

            var groups = await _groupService.SearchGroupsAsync(searchTerm, page, pageSize);
            return Ok(groups);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching groups with term '{searchTerm}': {ex.Message}");
            return StatusCode(500, new { Error = "An error occurred while searching for groups." });
        }
    }
}