using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryResponse> GetCategoriesAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Subcategories)
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var categoryDtos = new List<CategoryDto>();

        foreach (var cat in categories)
        {
            var gigCount = await _context.Gigs
                .CountAsync(g => g.CategoryId == cat.Id || 
                            (g.Category != null && g.Category.ParentId == cat.Id));

            categoryDtos.Add(new CategoryDto
            {
                Id = cat.Id,
                Name = cat.Name,
                Slug = cat.Slug,
                Icon = cat.Icon,
                Image = cat.ImageUrl,
                GigCount = gigCount,
                Subcategories = cat.Subcategories.Select(s => new SubcategoryDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Slug = s.Slug
                }).ToList()
            });
        }

        return new CategoryResponse
        {
            Success = true,
            Data = categoryDtos
        };
    }

    public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
    {
        var category = await _context.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category == null) return null;

        var gigCount = await _context.Gigs
            .CountAsync(g => g.CategoryId == category.Id);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Icon = category.Icon,
            Image = category.ImageUrl,
            GigCount = gigCount,
            Subcategories = category.Subcategories.Select(s => new SubcategoryDto
            {
                Id = s.Id,
                Name = s.Name,
                Slug = s.Slug
            }).ToList()
        };
    }
}

