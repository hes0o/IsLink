using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("gig_images")]
public class GigImage
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("gig_id")]
    public Guid GigId { get; set; }

    [Required]
    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("is_primary")]
    public bool IsPrimary { get; set; } = false;

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    // Navigation
    [ForeignKey("GigId")]
    public virtual Gig Gig { get; set; } = null!;
}

