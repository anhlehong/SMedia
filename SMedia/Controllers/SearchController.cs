// using Application.DTOs;
// using Application.Interfaces;
// using Application.Interfaces.ServiceInterfaces;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Presentation.Controllers;
//
// [ApiController]
// [Route("api/[controller]")]
// [Authorize]
// public class SearchController : ControllerBase
// {
//     private readonly ISearchService _searchService;
//
//     public SearchController(ISearchService searchService)
//     {
//         _searchService = searchService;
//     }
//
//     [HttpGet("users")]
//     public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] int skip = 0, [FromQuery] int take = 10)
//     {
//         var users = await _searchService.SearchUsersAsync(query, skip, take);
//         return Ok(users);
//     }
//
//     [HttpGet("random-users")]
//     public async Task<IActionResult> GetRandomUsers([FromQuery] Guid currentUserId, [FromQuery] int count = 5)
//     {
//         var users = await _searchService.GetRandomUsersAsync(currentUserId, count);
//         return Ok(users);
//     }
//
//     [HttpPost("advanced-users")]
//     public async Task<IActionResult> AdvancedSearchUsers([FromBody] AdvancedSearchDto searchDto, 
//         [FromQuery] int skip = 0, [FromQuery] int take = 10)
//     {
//         var users = await _searchService.AdvancedSearchUsersAsync(searchDto, skip, take);
//         return Ok(users);
//     }
// }
using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _searchService.SearchUsersAsync(query, page, pageSize);
        return Ok(users);
    }

    [HttpGet("random-users")]
    public async Task<IActionResult> GetRandomUsers([FromQuery] Guid currentUserId, [FromQuery] int count = 5)
    {
        var users = await _searchService.GetRandomUsersAsync(currentUserId, count);
        return Ok(users);
    }

    [HttpPost("advanced-users")]
    public async Task<IActionResult> AdvancedSearchUsers([FromBody] AdvancedSearchDto searchDto, 
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _searchService.AdvancedSearchUsersAsync(searchDto, page, pageSize);
        return Ok(users);
    }
}