using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Get all conversations for current user
    /// </summary>
    [HttpGet("conversations")]
    public async Task<ActionResult<ConversationListResponse>> GetConversations()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _messageService.GetConversationsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Get or create conversation with another user
    /// </summary>
    [HttpPost("conversations")]
    public async Task<ActionResult> GetOrCreateConversation([FromBody] CreateConversationRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var (success, data) = await _messageService.GetOrCreateConversationAsync(userId, request);
        
        if (!success)
            return BadRequest(new { Success = false, Message = "Failed to create conversation" });
        
        return Ok(new { Success = true, Data = data });
    }

    /// <summary>
    /// Get messages for a conversation
    /// </summary>
    [HttpGet("conversations/{conversationId}")]
    public async Task<ActionResult<MessageListResponse>> GetMessages(
        string conversationId,
        [FromQuery] int limit = 50,
        [FromQuery] DateTime? before = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _messageService.GetMessagesAsync(userId, conversationId, limit, before);
        
        if (!result.Success)
            return NotFound(new { Success = false, Message = "Conversation not found" });
        
        return Ok(result);
    }

    /// <summary>
    /// Send a message in a conversation
    /// </summary>
    [HttpPost("conversations/{conversationId}")]
    public async Task<ActionResult> SendMessage(string conversationId, [FromBody] SendMessageRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var (success, data) = await _messageService.SendMessageAsync(userId, conversationId, request);
        
        if (!success)
            return BadRequest(new { Success = false, Message = "Failed to send message" });
        
        return CreatedAtAction(nameof(GetMessages), new { conversationId }, new { Success = true, Data = data });
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

