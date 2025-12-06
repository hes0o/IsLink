using Microsoft.AspNetCore.Mvc; 
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using FreelancerPlatform.Entities;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Repositories.Interfaces;


[Authorize]
[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repo;
    private readonly IMapper _mapper;

    public NotificationsController(INotificationRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetMyNotifications()
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var notifications = await _repo.GetForUserAsync(userId);
        return _mapper.Map<List<NotificationDto>>(notifications);
    }
}
