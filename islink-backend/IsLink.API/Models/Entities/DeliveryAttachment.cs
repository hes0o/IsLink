using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("delivery_attachments")]
public class DeliveryAttachment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("delivery_id")]
    public Guid DeliveryId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("file_name")]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [Column("file_url")]
    public string FileUrl { get; set; } = string.Empty;

    [Column("file_size")]
    public int? FileSize { get; set; }

    [MaxLength(100)]
    [Column("file_type")]
    public string? FileType { get; set; }

    // Navigation
    [ForeignKey("DeliveryId")]
    public virtual OrderDelivery Delivery { get; set; } = null!;
}

