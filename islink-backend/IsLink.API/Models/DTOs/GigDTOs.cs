using System.ComponentModel.DataAnnotations;

namespace IsLink.API.Models.DTOs;

// ============================================
// Request DTOs
// ============================================

public class CreateGigRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public Guid? CategoryId { get; set; }

    public List<string> Images { get; set; } = new();

    public List<string> Tags { get; set; } = new();

    [Required]
    public GigPackagesDto Packages { get; set; } = new();
}

public class GigPackagesDto
{
    [Required]
    public PackageDto Basic { get; set; } = new();

    [Required]
    public PackageDto Standard { get; set; } = new();

    [Required]
    public PackageDto Premium { get; set; } = new();
}

public class PackageDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int DeliveryDays { get; set; }

    [Required]
    public string Revisions { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<string> Features { get; set; } = new();
}

public class GigFilterRequest
{
    public string? Category { get; set; }
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? DeliveryTime { get; set; }
    public string SortBy { get; set; } = "recommended";
    public int Limit { get; set; } = 20;
    public int Offset { get; set; } = 0;
}

// ============================================
// Response DTOs
// ============================================

public class GigListResponse
{
    public bool Success { get; set; }
    public List<GigSummaryDto> Data { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

public class GigDetailResponse
{
    public bool Success { get; set; }
    public GigDto? Data { get; set; }
}

public class GigSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public int OrdersInQueue { get; set; }
    public List<string> Images { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public GigPackagesSummaryDto Packages { get; set; } = new();
    public CategorySummaryDto? Category { get; set; }
    public SellerSummaryDto Seller { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class GigDto : GigSummaryDto
{
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class GigPackagesSummaryDto
{
    public PackageSummaryDto? Basic { get; set; }
    public PackageSummaryDto? Standard { get; set; }
    public PackageSummaryDto? Premium { get; set; }
}

public class PackageSummaryDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DeliveryDays { get; set; }
    public string Revisions { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Features { get; set; } = new();
}

public class CategorySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class SellerSummaryDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOnline { get; set; }
    public string? Country { get; set; }
    public string? Bio { get; set; }
    public int CompletedOrders { get; set; }
    public DateTime MemberSince { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<LanguageDto> Languages { get; set; } = new();
}

public class PaginationDto
{
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Total { get; set; }
}

