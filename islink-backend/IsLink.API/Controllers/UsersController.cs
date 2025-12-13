using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get list of sellers
    /// </summary>
    [HttpGet("sellers")]
    public async Task<ActionResult> GetSellers(
        [FromQuery] string? skill,
        [FromQuery] string? country,
        [FromQuery] decimal? minRating,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var sellers = await _userService.GetSellersAsync(skill, country, minRating, limit, offset);
        return Ok(new { Success = true, Data = sellers });
    }

    /// <summary>
    /// Get user profile by username
    /// </summary>
    [HttpGet("{username}")]
    public async Task<ActionResult> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        
        if (user == null)
            return NotFound(new { Success = false, Message = "User not found" });
        
        return Ok(new { Success = true, Data = user });
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var user = await _userService.UpdateProfileAsync(userId.Value, request);
        
        if (user == null)
            return NotFound(new { Success = false, Message = "User not found" });
        
        return Ok(new { Success = true, Message = "Profile updated", Data = user });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

