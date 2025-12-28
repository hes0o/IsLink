using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LinkerAIController : ControllerBase
{
    private readonly ILinkerAIService _linkerAIService;
    private readonly IConfiguration _configuration;

    public LinkerAIController(ILinkerAIService linkerAIService, IConfiguration configuration)
    {
        _linkerAIService = linkerAIService;
        _configuration = configuration;
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


    /// <summary>
    /// Check for an active session
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<LinkerAIChatResponse>> GetActiveSession()
    {
        try
        {
            var userId = GetCurrentUserId()?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                // If not logged in or can't identify user, no active session
                return NoContent();
            }

            var result = await _linkerAIService.GetActiveSessionAsync(userId);
            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get list of past sessions for the user
    /// </summary>
    [HttpGet("sessions")]
    public async Task<ActionResult<List<ChatSessionDto>>> GetSessions()
    {
        try
        {
            var userId = GetCurrentUserId()?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var sessions = await _linkerAIService.GetUserSessionsAsync(userId);
            return Ok(new { Success = true, Data = sessions });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get details (messages) of a specific session
    /// </summary>
    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<ChatSessionDetailDto>> GetSession(string sessionId)
    {
        try
        {
            var userId = GetCurrentUserId()?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var session = await _linkerAIService.GetSessionAsync(sessionId, userId);
            if (session == null)
            {
                return NotFound(new { Success = false, Message = "Session not found" });
            }

            return Ok(new { Success = true, Data = session });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Debug/status endpoint to verify Groq API is configured.
    /// </summary>
    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        var groqKey =
            _configuration["Groq:ApiKey"] ??
            Environment.GetEnvironmentVariable("Groq__ApiKey") ??
            Environment.GetEnvironmentVariable("GROQ_API_KEY");

        return Ok(new
        {
            Success = true,
            ProviderMode = "groq",
            GroqConfigured = !string.IsNullOrWhiteSpace(groqKey),
            Timestamp = DateTime.UtcNow
        });
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
