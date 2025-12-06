using System.ComponentModel.DataAnnotations;
using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.DTOs
{
    // DTO for registering a new user.
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string DisplayName { get; set; } = string.Empty; // Used to create the initial profile.

        public UserRole Role { get; set; } // Defines if the user is a Client or a Freelancer.
    }

    // DTO for user login.
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // DTO for returning user details to the client (Frontend).
    // Note: We never return the PasswordHash here.
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; }
        public string Token { get; set; } = string.Empty; // JWT Token for authentication.
    }
}