using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("favorites")]
public class Favorite
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("gig_id")]
    public Guid GigId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual Gig Gig { get; set; } = null!;
}

