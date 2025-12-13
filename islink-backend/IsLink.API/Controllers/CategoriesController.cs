using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace IsLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories with subcategories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CategoryResponse>> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get category by slug
    /// </summary>
    [HttpGet("{slug}")]
    public async Task<ActionResult> GetCategoryBySlug(string slug)
    {
        var category = await _categoryService.GetCategoryBySlugAsync(slug);
        
        if (category == null)
            return NotFound(new { Success = false, Message = "Category not found" });
        
        return Ok(new { Success = true, Data = category });
    }
}

