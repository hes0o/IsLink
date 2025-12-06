using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPlatform.Entities
{
    // Represents a dispute/conflict raised by a user regarding an order.
    public class Dispute
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Open"; // e.g., Open, Resolved.
    }
}