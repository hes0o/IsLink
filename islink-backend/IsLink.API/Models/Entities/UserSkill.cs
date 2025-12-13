using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

[Table("user_skills")]
public class UserSkill
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("skill_name")]
    public string SkillName { get; set; } = string.Empty;

    // Navigation
    public virtual User User { get; set; } = null!;
}

