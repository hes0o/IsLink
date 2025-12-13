using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("gig_packages")]
public class GigPackage
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("gig_id")]
    public Guid GigId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("package_type")]
    public string PackageType { get; set; } = string.Empty; // basic, standard, premium

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("price")]
    public decimal Price { get; set; } // Syrian Lira

    [Column("delivery_days")]
    public int DeliveryDays { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("revisions")]
    public string Revisions { get; set; } = string.Empty; // Number or "Unlimited"

    [Column("description")]
    public string? Description { get; set; }

    // Navigation Properties
    [ForeignKey("GigId")]
    public virtual Gig Gig { get; set; } = null!;

    public virtual ICollection<PackageFeature> Features { get; set; } = new List<PackageFeature>();
}

