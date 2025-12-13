using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface ICategoryService
{
    Task<CategoryResponse> GetCategoriesAsync();
    Task<CategoryDto?> GetCategoryBySlugAsync(string slug);
}

