using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LinkerAIController : ControllerBase
{
    private readonly ILinkerAIService _linkerAIService;

    public LinkerAIController(ILinkerAIService linkerAIService)
    {
        _linkerAIService = linkerAIService;
    }

    /// <summary>
    /// Start a new LinkerAI conversation
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<LinkerAIChatResponse>> StartConversation([FromBody] LinkerAIStartRequest request)
    {
        try
        {
            var userId = GetCurrentUserId()?.ToString();
            request.UserId = userId ?? request.UserId;

            var result = await _linkerAIService.StartConversationAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new LinkerAIChatResponse
            {
                Success = false,
                Message = $"Error starting conversation: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Send a message in an existing conversation
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<LinkerAIChatResponse>> SendMessage([FromBody] LinkerAIChatRequest request)
    {
        try
        {
            var userId = GetCurrentUserId()?.ToString();
            request.UserId = userId ?? request.UserId;

            var result = await _linkerAIService.SendMessageAsync(request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new LinkerAIChatResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new LinkerAIChatResponse
            {
                Success = false,
                Message = $"Error processing message: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Get recommendations for a session (if already generated)
    /// </summary>
    [HttpGet("recommendations/{sessionId}")]
    public async Task<ActionResult<LinkerAIRecommendations>> GetRecommendations(string sessionId)
    {
        try
        {
            var recommendations = await _linkerAIService.GetRecommendationsAsync(sessionId);
            
            if (recommendations == null)
            {
                return NotFound(new { Success = false, Message = "Recommendations not found for this session" });
            }

            return Ok(new { Success = true, Data = recommendations });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}
