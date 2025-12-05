using System.ComponentModel.DataAnnotations;

namespace FreelancerPlatform.Entities
{
    // Represents a single skill (e.g., "Web Development", "Translation") linked to a profile.
    public class Skill
    {
        public int Id { get; set; }

        // Foreign Key to the Profile table.
        public int ProfileId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}