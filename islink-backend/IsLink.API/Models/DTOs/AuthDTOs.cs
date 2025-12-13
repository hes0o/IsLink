using System.ComponentModel.DataAnnotations;

namespace IsLink.API.Models.DTOs;

// ============================================
// Request DTOs
// ============================================

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public string AccountType { get; set; } = "buyer"; // buyer or seller
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

// ============================================
// Response DTOs
// ============================================

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string? Token { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Bio { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public int CompletedOrders { get; set; }
    public bool IsOnline { get; set; }
    public bool IsVerified { get; set; }
    public decimal Balance { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<LanguageDto> Languages { get; set; } = new();
    public DateTime MemberSince { get; set; }
}

public class LanguageDto
{
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

