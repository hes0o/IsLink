using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("user_languages")]
public class UserLanguage
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("language_name")]
    public string LanguageName { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("proficiency")]
    public string? Proficiency { get; set; } // Basic, Conversational, Fluent, Native

    // Navigation
    public virtual User User { get; set; } = null!;
}

