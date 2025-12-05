using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPlatform.Entities
{
    // Represents a work submission by the freelancer for a specific order.
    public class Delivery
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        public string? Message { get; set; } // Note from freelancer.
        public string? AttachedFiles { get; set; } // URL or path to the uploaded files.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}