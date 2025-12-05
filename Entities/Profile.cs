using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPlatform.Entities
{
    // Represents the public profile of a user.
    public class Profile
    {
        public int Id { get; set; }

        // Foreign Key linking back to the User table.
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property to access the User object.
        public User User { get; set; } = null!;

        // The public name shown on the platform.
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        public string? Bio { get; set; } // Short biography.
        public string? AvatarUrl { get; set; } // URL to profile picture.
        public string? Country { get; set; }

        // A profile can have multiple skills (e.g., C#, Design).
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}