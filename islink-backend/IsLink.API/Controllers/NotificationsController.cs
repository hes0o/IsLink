using IsLink.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get user's notifications
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetNotifications(
        [FromQuery] int limit = 20,
        [FromQuery] int skip = 0)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync(userId, limit, skip);
        var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

        return Ok(new 
        { 
            Success = true, 
            Data = notifications.Select(n => new
            {
                n.Id,
                n.Type,
                n.Title,
                n.Message,
                n.Data,
                n.IsRead,
                n.CreatedAt
            }),
            UnreadCount = unreadCount
        });
    }

    /// <summary>
    /// Get unread notification count
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new { Success = true, Count = count });
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("mark-all-read")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(new { Success = true, Message = "All notifications marked as read" });
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

