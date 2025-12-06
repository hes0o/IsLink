using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using FreelancerPlatform.Entities;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Repositories.Interfaces;
using FreelancerPlatform.Data;
using EntityProfile = FreelancerPlatform.Entities.Profile;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileRepository _profileRepo;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProfileController(IProfileRepository profileRepo, AppDbContext context, IMapper mapper)
    {
        _profileRepo = profileRepo;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("me")]
    public async Task<ActionResult<ProfileDto>> GetMyProfile()
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null) return NotFound("Profile not found.");

        return _mapper.Map<ProfileDto>(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null) return NotFound("Profile not found.");

        profile.Bio = dto.Bio;
        profile.AvatarUrl = dto.AvatarUrl;

        // Replace Skills
        profile.Skills.Clear();
        foreach (var s in dto.Skills)
        {
            profile.Skills.Add(new Skill { Name = s, ProfileId = profile.Id });
        }

        await _profileRepo.SaveChangesAsync();
        return NoContent();
    }
}
