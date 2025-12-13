using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(20)]
    [Column("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Column("gig_id")]
    public Guid? GigId { get; set; }

    [Column("buyer_id")]
    public Guid? BuyerId { get; set; }

    [Column("seller_id")]
    public Guid? SellerId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("package_type")]
    public string PackageType { get; set; } = string.Empty;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("service_fee")]
    public decimal ServiceFee { get; set; }

    [Column("total_price")]
    public decimal TotalPrice { get; set; }

    [MaxLength(30)]
    [Column("status")]
    public string Status { get; set; } = "pending"; // pending, in_progress, delivered, completed, cancelled, revision_requested

    [Column("requirements")]
    public string? Requirements { get; set; }

    [Column("delivery_deadline")]
    public DateTime? DeliveryDeadline { get; set; }

    [Column("delivered_at")]
    public DateTime? DeliveredAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("GigId")]
    public virtual Gig? Gig { get; set; }

    [ForeignKey("BuyerId")]
    public virtual User? Buyer { get; set; }

    [ForeignKey("SellerId")]
    public virtual User? Seller { get; set; }

    public virtual ICollection<OrderDelivery> Deliveries { get; set; } = new List<OrderDelivery>();
    public virtual Review? Review { get; set; }
}

