using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Column("gig_id")]
    public Guid? GigId { get; set; }

    [Column("buyer_id")]
    public Guid? BuyerId { get; set; }

    [Column("seller_id")]
    public Guid? SellerId { get; set; }

    [Range(1, 5)]
    [Column("rating")]
    public int Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("seller_response")]
    public string? SellerResponse { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("GigId")]
    public virtual Gig? Gig { get; set; }

    [ForeignKey("BuyerId")]
    public virtual User? Buyer { get; set; }

    [ForeignKey("SellerId")]
    public virtual User? Seller { get; set; }
}

