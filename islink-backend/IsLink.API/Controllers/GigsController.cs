using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GigsController : ControllerBase
{
    private readonly IGigService _gigService;

    public GigsController(IGigService gigService)
    {
        _gigService = gigService;
    }

    /// <summary>
    /// Get all gigs with filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GigListResponse>> GetGigs([FromQuery] GigFilterRequest filter)
    {
        var result = await _gigService.GetGigsAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Get gigs by seller username
    /// </summary>
    [HttpGet("seller/{username}")]
    public async Task<ActionResult<GigListResponse>> GetGigsBySeller(string username)
    {
        var result = await _gigService.GetGigsBySellerAsync(username);
        return Ok(result);
    }

    /// <summary>
    /// Get single gig by slug
    /// </summary>
    [HttpGet("{slug}")]
    public async Task<ActionResult<GigDetailResponse>> GetGigBySlug(string slug)
    {
        var result = await _gigService.GetGigBySlugAsync(slug);
        
        if (!result.Success)
            return NotFound(new { Success = false, Message = "Gig not found" });
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new gig (sellers only)
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<GigDetailResponse>> CreateGig([FromBody] CreateGigRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        // TODO: Check if user is a seller

        var result = await _gigService.CreateGigAsync(userId.Value, request);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetGigBySlug), new { slug = result.Data?.Slug }, result);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

