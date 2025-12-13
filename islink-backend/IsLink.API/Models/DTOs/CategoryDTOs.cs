namespace IsLink.API.Models.DTOs;

public class CategoryResponse
{
    public bool Success { get; set; }
    public List<CategoryDto> Data { get; set; } = new();
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Image { get; set; }
    public List<SubcategoryDto> Subcategories { get; set; } = new();
    public int GigCount { get; set; }
}

public class SubcategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

