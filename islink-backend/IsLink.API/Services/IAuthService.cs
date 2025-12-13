using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> GetCurrentUserAsync(Guid userId);
    Task LogoutAsync(Guid userId);
    string GenerateJwtToken(Guid userId);
}

