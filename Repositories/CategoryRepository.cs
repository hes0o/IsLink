using Microsoft.EntityFrameworkCore;
using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;

namespace FreelancerPlatform.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync(); 
        }
    }
}