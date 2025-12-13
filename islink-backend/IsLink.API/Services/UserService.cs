using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _context.Users
            .Include(u => u.Skills)
            .Include(u => u.Languages)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            Country = user.Country,
            Bio = user.Bio,
            Rating = user.Rating,
            ReviewCount = user.ReviewCount,
            CompletedOrders = user.CompletedOrders,
            IsOnline = user.IsOnline,
            IsVerified = user.IsVerified,
            Skills = user.Skills.Select(s => s.SkillName).ToList(),
            Languages = user.Languages.Select(l => new LanguageDto 
            { 
                Name = l.LanguageName, 
                Level = l.Proficiency ?? "" 
            }).ToList(),
            MemberSince = user.CreatedAt
        };
    }

    public async Task<UserDto?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Skills)
            .Include(u => u.Languages)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        // Update basic info
        if (!string.IsNullOrEmpty(request.FullName)) user.FullName = request.FullName;
        if (!string.IsNullOrEmpty(request.Bio)) user.Bio = request.Bio;
        if (!string.IsNullOrEmpty(request.Country)) user.Country = request.Country;
        if (!string.IsNullOrEmpty(request.AvatarUrl)) user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        // Update skills
        if (request.Skills != null)
        {
            _context.UserSkills.RemoveRange(user.Skills);
            user.Skills = request.Skills.Select(s => new UserSkill 
            { 
                UserId = userId, 
                SkillName = s 
            }).ToList();
        }

        // Update languages
        if (request.Languages != null)
        {
            _context.UserLanguages.RemoveRange(user.Languages);
            user.Languages = request.Languages.Select(l => new UserLanguage 
            { 
                UserId = userId, 
                LanguageName = l.Name,
                Proficiency = l.Level
            }).ToList();
        }

        await _context.SaveChangesAsync();

        return await GetUserByUsernameAsync(user.Username);
    }

    public async Task<List<SellerSummaryDto>> GetSellersAsync(string? skill, string? country, decimal? minRating, int limit, int offset)
    {
        var query = _context.Users
            .Include(u => u.Skills)
            .Where(u => u.Role == "seller")
            .AsQueryable();

        if (!string.IsNullOrEmpty(skill))
        {
            query = query.Where(u => u.Skills.Any(s => s.SkillName.ToLower().Contains(skill.ToLower())));
        }

        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(u => u.Country != null && u.Country.ToLower().Contains(country.ToLower()));
        }

        if (minRating.HasValue)
        {
            query = query.Where(u => u.Rating >= minRating.Value);
        }

        var sellers = await query
            .OrderByDescending(u => u.Rating)
            .ThenByDescending(u => u.ReviewCount)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return sellers.Select(u => new SellerSummaryDto
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            AvatarUrl = u.AvatarUrl,
            Rating = u.Rating,
            ReviewCount = u.ReviewCount,
            IsVerified = u.IsVerified,
            IsOnline = u.IsOnline,
            Country = u.Country,
            CompletedOrders = u.CompletedOrders,
            Skills = u.Skills.Select(s => s.SkillName).ToList()
        }).ToList();
    }
}

