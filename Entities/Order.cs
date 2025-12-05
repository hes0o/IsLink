using System.ComponentModel.DataAnnotations.Schema;
using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.Entities
{
    // Represents a purchased service (Order).
    public class Order
    {
        public int Id { get; set; }

        // The User who bought the service (Client).
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public User Client { get; set; } = null!;

        // The User who is performing the service (Freelancer).
        public int FreelancerId { get; set; }
        [ForeignKey("FreelancerId")]
        public User Freelancer { get; set; } = null!;

        // The specific Gig and Package purchased.
        public int GigId { get; set; }
        public Gig Gig { get; set; } = null!;

        public int PackageId { get; set; }

        // Current state of the order (Pending, InProgress, Completed, etc.).
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } // Deadline for the freelancer.

        // --- Relationships ---
        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public Review? Review { get; set; }
        public Dispute? Dispute { get; set; }
    }
}