using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.Entities
{
    // Represents a service (Gig) posted by a freelancer.
    public class Gig
    {
        public int Id { get; set; }

        // The Freelancer who owns this Gig.
        public int FreelancerId { get; set; }
        [ForeignKey("FreelancerId")]
        public User Freelancer { get; set; } = null!;

        // The Category this Gig belongs to.
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Status of the Gig (Draft, Active, Paused).
        public GigStatus Status { get; set; } = GigStatus.Draft;

        // Calculated fields for ratings.
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // A Gig can have different pricing tiers (Packages).
        public ICollection<GigPackage> Packages { get; set; } = new List<GigPackage>();
    }
}