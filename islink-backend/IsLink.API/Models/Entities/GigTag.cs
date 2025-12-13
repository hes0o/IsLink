using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("gig_tags")]
public class GigTag
{
    [Column("gig_id")]
    public Guid GigId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tag")]
    public string Tag { get; set; } = string.Empty;

    // Navigation
    public virtual Gig Gig { get; set; } = null!;
}

