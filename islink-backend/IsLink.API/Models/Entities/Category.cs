using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("categories")]
public class Category
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(10)]
    [Column("icon")]
    public string? Icon { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("parent_id")]
    public Guid? ParentId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("ParentId")]
    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> Subcategories { get; set; } = new List<Category>();
    public virtual ICollection<Gig> Gigs { get; set; } = new List<Gig>();
}

