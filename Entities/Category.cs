using System.ComponentModel.DataAnnotations;

namespace FreelancerPlatform.Entities
{
    // Represents a category to organize Gigs.
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // One-to-Many: A category contains many Gigs.
        public ICollection<Gig> Gigs { get; set; } = new List<Gig>();
    }
}