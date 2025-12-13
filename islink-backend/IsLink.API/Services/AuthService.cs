using IsLink.API.Configuration;
using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IsLink.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    // Demo accounts for testing when database is not connected
    private static readonly Dictionary<string, (string Password, UserDto User)> DemoAccounts = new()
    {
        ["design@example.com"] = ("password123", new UserDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "design@example.com",
            Username = "design_pro",
            FullName = "Sarah Designer",
            AvatarUrl = "https://randomuser.me/api/portraits/women/44.jpg",
            Role = "seller",
            Country = "Syria",
            Bio = "Professional graphic designer with 5+ years of experience in logo design, branding, and UI/UX.",
            Rating = 4.9m,
            ReviewCount = 127,
            CompletedOrders = 156,
            IsOnline = true,
            IsVerified = true,
            Balance = 2450.00m,
            Skills = new List<string> { "Logo Design", "Branding", "UI/UX", "Photoshop", "Illustrator" },
            Languages = new List<LanguageDto> 
            { 
                new() { Name = "Arabic", Level = "Native" },
                new() { Name = "English", Level = "Fluent" }
            },
            MemberSince = DateTime.UtcNow.AddYears(-2)
        }),
        ["buyer@example.com"] = ("password123", new UserDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Email = "buyer@example.com",
            Username = "john_buyer",
            FullName = "John Smith",
            AvatarUrl = "https://randomuser.me/api/portraits/men/32.jpg",
            Role = "buyer",
            Country = "United States",
            Bio = "Entrepreneur looking for talented freelancers for various projects.",
            Rating = 4.8m,
            ReviewCount = 23,
            CompletedOrders = 45,
            IsOnline = true,
            IsVerified = true,
            Balance = 500.00m,
            Skills = new List<string>(),
            Languages = new List<LanguageDto> 
            { 
                new() { Name = "English", Level = "Native" }
            },
            MemberSince = DateTime.UtcNow.AddYears(-1)
        }),
        ["dev@example.com"] = ("password123", new UserDto
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Email = "dev@example.com",
            Username = "code_master",
            FullName = "Ahmed Developer",
            AvatarUrl = "https://randomuser.me/api/portraits/men/75.jpg",
            Role = "seller",
            Country = "Syria",
            Bio = "Full-stack developer specializing in React, Node.js, and .NET. Building amazing web applications.",
            Rating = 5.0m,
            ReviewCount = 89,
            CompletedOrders = 112,
            IsOnline = true,
            IsVerified = true,
            Balance = 3200.00m,
            Skills = new List<string> { "React", "Node.js", ".NET", "TypeScript", "PostgreSQL", "MongoDB" },
            Languages = new List<LanguageDto> 
            { 
                new() { Name = "Arabic", Level = "Native" },
                new() { Name = "English", Level = "Fluent" },
                new() { Name = "Turkish", Level = "Conversational" }
            },
            MemberSince = DateTime.UtcNow.AddYears(-3)
        })
    };

    public AuthService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    private async Task<bool> IsDatabaseAvailable()
    {
        try
        {
            return await _context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if database is available
        if (!await IsDatabaseAvailable())
        {
            // Demo mode - simulate registration
            var demoUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLower(),
                Username = request.Username.ToLower(),
                FullName = request.FullName,
                Role = request.AccountType,
                AvatarUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(request.FullName)}&background=1dbf73&color=fff",
                IsOnline = true,
                MemberSince = DateTime.UtcNow,
                Skills = new List<string>(),
                Languages = new List<LanguageDto>()
            };

            var token = GenerateJwtToken(demoUser.Id);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful (Demo Mode)",
                User = demoUser,
                Token = token
            };
        }

        // Normal database mode
        var existingUser = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() || 
                          u.Username.ToLower() == request.Username.ToLower());

        if (existingUser)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User with this email or username already exists"
            };
        }

        var user = new User
        {
            Email = request.Email.ToLower(),
            Username = request.Username.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Role = request.AccountType
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var dbToken = GenerateJwtToken(user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful",
            User = MapToUserDto(user),
            Token = dbToken
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Check if database is available
        if (!await IsDatabaseAvailable())
        {
            Console.WriteLine("⚠️ Database not available - using demo mode");
            
            // Demo mode - check demo accounts
            var emailLower = request.Email.ToLower();
            
            if (DemoAccounts.TryGetValue(emailLower, out var demoAccount))
            {
                if (demoAccount.Password == request.Password)
                {
                    var token = GenerateJwtToken(demoAccount.User.Id);
                    
                    return new AuthResponse
                    {
                        Success = true,
                        Message = "Login successful (Demo Mode)",
                        User = demoAccount.User,
                        Token = token
                    };
                }
            }

            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password. Demo accounts: design@example.com, buyer@example.com, dev@example.com (password: password123)"
            };
        }

        // Normal database mode
        var user = await _context.Users
            .Include(u => u.Skills)
            .Include(u => u.Languages)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        user.IsOnline = true;
        await _context.SaveChangesAsync();

        var dbToken = GenerateJwtToken(user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            User = MapToUserDto(user),
            Token = dbToken
        };
    }

    public async Task<AuthResponse> GetCurrentUserAsync(Guid userId)
    {
        // Check if database is available
        if (!await IsDatabaseAvailable())
        {
            // Demo mode - return demo user
            var demoUser = DemoAccounts.Values.FirstOrDefault(a => a.User.Id == userId);
            if (demoUser.User != null)
            {
                return new AuthResponse
                {
                    Success = true,
                    User = demoUser.User
                };
            }

            return new AuthResponse
            {
                Success = false,
                Message = "User not found"
            };
        }

        var user = await _context.Users
            .Include(u => u.Skills)
            .Include(u => u.Languages)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User not found"
            };
        }

        return new AuthResponse
        {
            Success = true,
            User = MapToUserDto(user)
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        if (!await IsDatabaseAvailable())
        {
            return; // Nothing to do in demo mode
        }

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsOnline = false;
            await _context.SaveChangesAsync();
        }
    }

    public string GenerateJwtToken(Guid userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpirationInDays),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static UserDto MapToUserDto(User user)
    {
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
            Balance = user.Balance,
            Skills = user.Skills.Select(s => s.SkillName).ToList(),
            Languages = user.Languages.Select(l => new LanguageDto 
            { 
                Name = l.LanguageName, 
                Level = l.Proficiency ?? "" 
            }).ToList(),
            MemberSince = user.CreatedAt
        };
    }
}
