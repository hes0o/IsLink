using System.Collections.Generic;
using System.Threading.Tasks;
using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Repositories.Interfaces
{
    public interface ISearchRepository
    {
        // Search gigs with filters and pagination
        Task<(List<Gig> Items, int TotalCount)> SearchGigsAsync(
            string? query,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int pageNumber,
            int pageSize);
    }
}