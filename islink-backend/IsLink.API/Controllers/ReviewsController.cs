using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Get reviews for a gig
    /// </summary>
    [HttpGet("gig/{gigId:guid}")]
    public async Task<ActionResult<ReviewListResponse>> GetGigReviews(
        Guid gigId,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
    {
        var result = await _reviewService.GetGigReviewsAsync(gigId, limit, offset);
        return Ok(result);
    }

    /// <summary>
    /// Get reviews for a seller
    /// </summary>
    [HttpGet("seller/{username}")]
    public async Task<ActionResult<ReviewListResponse>> GetSellerReviews(
        string username,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
    {
        var result = await _reviewService.GetSellerReviewsAsync(username, limit, offset);
        
        if (!result.Success)
            return NotFound(new { Success = false, Message = "Seller not found" });
        
        return Ok(result);
    }

    /// <summary>
    /// Create a review for completed order
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var (success, message, data) = await _reviewService.CreateReviewAsync(userId.Value, request);
        
        if (!success)
            return BadRequest(new { Success = false, Message = message });
        
        return CreatedAtAction(nameof(GetGigReviews), new { gigId = data?.Id }, new { Success = true, Message = message, Data = data });
    }

    /// <summary>
    /// Add seller response to a review
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/response")]
    public async Task<ActionResult> AddSellerResponse(Guid id, [FromBody] AddSellerResponseRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var (success, message) = await _reviewService.AddSellerResponseAsync(userId.Value, id, request.Response);
        
        if (!success)
            return BadRequest(new { Success = false, Message = message });
        
        return Ok(new { Success = true, Message = message });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

