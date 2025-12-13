using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [MaxLength(20)]
    [Column("role")]
    public string Role { get; set; } = "buyer"; // buyer, seller, admin

    [MaxLength(100)]
    [Column("country")]
    public string? Country { get; set; }

    [Column("bio")]
    public string? Bio { get; set; }

    [Column("rating")]
    public decimal Rating { get; set; } = 0;

    [Column("review_count")]
    public int ReviewCount { get; set; } = 0;

    [Column("completed_orders")]
    public int CompletedOrders { get; set; } = 0;

    [Column("is_online")]
    public bool IsOnline { get; set; } = false;

    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;

    [Column("balance")]
    public decimal Balance { get; set; } = 0; // Syrian Lira

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<UserSkill> Skills { get; set; } = new List<UserSkill>();
    public virtual ICollection<UserLanguage> Languages { get; set; } = new List<UserLanguage>();
    public virtual ICollection<Gig> Gigs { get; set; } = new List<Gig>();
    public virtual ICollection<Order> BuyerOrders { get; set; } = new List<Order>();
    public virtual ICollection<Order> SellerOrders { get; set; } = new List<Order>();
    public virtual ICollection<Review> BuyerReviews { get; set; } = new List<Review>();
    public virtual ICollection<Review> SellerReviews { get; set; } = new List<Review>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}

