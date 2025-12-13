using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("transactions")]
public class Transaction
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("order_id")]
    public Guid? OrderId { get; set; }

    [MaxLength(30)]
    [Column("type")]
    public string Type { get; set; } = string.Empty; // payment, earning, withdrawal, refund, fee

    [Column("amount")]
    public decimal Amount { get; set; }

    [MaxLength(10)]
    [Column("currency")]
    public string Currency { get; set; } = "SYP"; // Syrian Lira

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "pending"; // pending, completed, failed

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }
}

