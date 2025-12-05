using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPlatform.Entities
{
    // Represents a system notification (e.g., "Order Update", "New Message").
    public class Notification
    {
        public int Id { get; set; }

        // The recipient of the notification.
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? Type { get; set; } // Categorizes the notification.
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}