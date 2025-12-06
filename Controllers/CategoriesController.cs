using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FreelancerPlatform.Entities;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Repositories.Interfaces;


[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var categories = await _repo.GetAllAsync();
        return _mapper.Map<List<CategoryDto>>(categories);
    }
}
