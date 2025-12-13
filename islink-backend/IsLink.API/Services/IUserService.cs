using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<UserDto?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<List<SellerSummaryDto>> GetSellersAsync(string? skill, string? country, decimal? minRating, int limit, int offset);
}

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? Country { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string>? Skills { get; set; }
    public List<LanguageDto>? Languages { get; set; }
}

