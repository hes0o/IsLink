using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPlatform.Entities
{
    // Represents a specific pricing option for a Gig.
    public class GigPackage
    {
        public int Id { get; set; }

        // Link to the parent Gig.
        public int GigId { get; set; }
        [ForeignKey("GigId")]
        public Gig Gig { get; set; } = null!;

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Basic Plan"

        // Defines the column type as decimal for currency precision.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int DeliveryTimeDays { get; set; } // How many days to deliver.
        public int Revisions { get; set; } // How many revisions are allowed.
    }
}