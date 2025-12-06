using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.DTOs
{
    // DTO sent by the Client to purchase a Gig.
    public class CreateOrderDto
    {
        public int GigId { get; set; }
        public int PackageId { get; set; }
        public string? NoteToFreelancer { get; set; } // Requirements or special instructions.
    }

    // Read-only DTO for viewing Order details in the dashboard.
    public class OrderDto
    {
        public int Id { get; set; }
        public string GigTitle { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;
        public string FreelancerName { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}