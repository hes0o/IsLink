using System;
using System.Collections.Generic;

namespace FreelancerPlatform.DTOs
{
    // 1. Input: Search parameters from the user
    public class GigSearchQueryDto
    {
        public string? Query { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public int PageNumber { get; set; } = 1;

        // Performance Improvement: Limit page size to prevent server overload
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 50) ? 50 : value;
        }
    }

    // 2. Output: The result object for a single Gig card
    public class GigSearchResultDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        // Price of the cheapest package
        public decimal PriceFrom { get; set; }

        public double Rating { get; set; }
        public string FreelancerName { get; set; } = string.Empty;
        public string? FreelancerAvatar { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    // 3. Output: Wrapper for pagination info + results
    public class PagedResultDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public List<T> Items { get; set; } = new List<T>();
    }
}