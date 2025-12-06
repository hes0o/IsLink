using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Essential for Include, Where, ToListAsync
using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;

namespace FreelancerPlatform.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly AppDbContext _context;

        public SearchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Gig> Items, int TotalCount)> SearchGigsAsync(
            string? query,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int pageNumber,
            int pageSize)
        {
            // 1. Performance: Use AsNoTracking for read-only queries
            var gigsQuery = _context.Gigs
                .AsNoTracking()
                .Include(g => g.Category)
                .Include(g => g.Freelancer).ThenInclude(u => u.Profile)
                .Include(g => g.Packages) // Needed for price filtering
                .Include(g => g.Images)   // Needed for cover image
                .AsQueryable();

            // 2. Apply Filters
            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                gigsQuery = gigsQuery.Where(g =>
                    g.Title.ToLower().Contains(q) ||
                    (g.Description != null && g.Description.ToLower().Contains(q)));
            }

            if (categoryId.HasValue)
            {
                gigsQuery = gigsQuery.Where(g => g.CategoryId == categoryId.Value);
            }

            // Price Filter: Check if ANY package falls within the range
            if (minPrice.HasValue)
            {
                gigsQuery = gigsQuery.Where(g => g.Packages.Any(p => p.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                gigsQuery = gigsQuery.Where(g => g.Packages.Any(p => p.Price <= maxPrice.Value));
            }

            // 3. Apply Sorting
            gigsQuery = sortBy switch
            {
                // Sort by the cheapest package price
                "price_asc" => gigsQuery.OrderBy(g => g.Packages.Min(p => p.Price)),
                "price_desc" => gigsQuery.OrderByDescending(g => g.Packages.Min(p => p.Price)),
                "rating" => gigsQuery.OrderByDescending(g => g.AverageRating),
                _ => gigsQuery.OrderByDescending(g => g.CreatedAt) // Default: Newest
            };

            // 4. Pagination
            var totalCount = await gigsQuery.CountAsync();

            var items = await gigsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}