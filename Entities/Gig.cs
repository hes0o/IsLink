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

        // --- Fiverr Features Updates ---

        // The question/requirements the freelancer asks the client to start the order.
        public string? Requirements { get; set; }

        // Calculated fields for ratings.
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Relationships ---

        // A Gig can have different pricing tiers (Packages).
        public ICollection<GigPackage> Packages { get; set; } = new List<GigPackage>();

        // Collection of images (Gallery) for the Gig.
        public ICollection<GigImage> Images { get; set; } = new List<GigImage>();

        // Search tags associated with the Gig for better visibility.
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}