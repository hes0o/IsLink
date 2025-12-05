using System.ComponentModel.DataAnnotations;
using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.DTOs
{
    // DTO used by a Freelancer to create a new Gig.
    public class CreateGigDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public string? Description { get; set; }

        // A Gig must have at least one pricing package.
        public List<CreateGigPackageDto> Packages { get; set; } = new List<CreateGigPackageDto>();
    }

    // Helper DTO for defining a package during Gig creation.
    public class CreateGigPackageDto
    {
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Basic", "Premium"

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int DeliveryTimeDays { get; set; }

        public int Revisions { get; set; }
    }

    // Read-only DTO for displaying a Gig on the website.
    public class GigDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // We return names instead of IDs for display purposes.
        public string CategoryName { get; set; } = string.Empty;
        public string FreelancerName { get; set; } = string.Empty;
        public string? FreelancerAvatar { get; set; }

        public double AverageRating { get; set; }

        public List<GigPackageDto> Packages { get; set; } = new List<GigPackageDto>();
    }

    // Read-only DTO for displaying packages inside a Gig.
    public class GigPackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DeliveryTimeDays { get; set; }
    }
}