using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("package_features")]
public class PackageFeature
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("package_id")]
    public Guid PackageId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("feature_text")]
    public string FeatureText { get; set; } = string.Empty;

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    // Navigation
    [ForeignKey("PackageId")]
    public virtual GigPackage Package { get; set; } = null!;
}

