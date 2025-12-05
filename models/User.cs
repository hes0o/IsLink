using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.Entities
{
    // Represents a registered user in the system (can be a Client or a Freelancer).
    public class User
    {
        // Primary Key in the database.
        public int Id { get; set; }

        // The user's email address. [EmailAddress] validates the format.
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Stores the encrypted/hashed password, not the plain text.
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Enum to define if the user is a Client, Freelancer, or Admin.
        public UserRole Role { get; set; }

        // Timestamps for audit purposes.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // --- Navigation Properties (Relationships) ---

        // One-to-One relationship: A user may have one profile.
        public Profile? Profile { get; set; }

        // One-to-Many: A freelancer can create multiple Gigs.
        public ICollection<Gig> Gigs { get; set; } = new List<Gig>();

        // One-to-Many: Orders placed by this user (as a Buyer/Client).
        // [InverseProperty] tells EF Core this list maps to the "Client" property in the Order class.
        [InverseProperty("Client")]
        public ICollection<Order> ClientOrders { get; set; } = new List<Order>();

        // One-to-Many: Orders received by this user (as a Seller/Freelancer).
        [InverseProperty("Freelancer")]
        public ICollection<Order> FreelancerOrders { get; set; } = new List<Order>();

        // One-to-Many: Notifications received by the user.
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}