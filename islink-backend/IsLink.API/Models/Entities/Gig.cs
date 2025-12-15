using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("gigs")]
public class Gig
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("seller_id")]
    public Guid SellerId { get; set; }

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("rating")]
    public decimal Rating { get; set; } = 0;

    [Column("review_count")]
    public int ReviewCount { get; set; } = 0;

    [Column("orders_in_queue")]
    public int OrdersInQueue { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("SellerId")]
    public virtual User Seller { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    public virtual ICollection<GigPackage> Packages { get; set; } = new List<GigPackage>();
    public virtual ICollection<GigImage> Images { get; set; } = new List<GigImage>();
    public virtual ICollection<GigTag> Tags { get; set; } = new List<GigTag>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    
}

