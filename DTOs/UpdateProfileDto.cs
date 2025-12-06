public class UpdateProfileDto
{
    public string Bio { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public List<string> Skills { get; set; } = new();
}
