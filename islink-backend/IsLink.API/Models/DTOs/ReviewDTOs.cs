using System.ComponentModel.DataAnnotations;

namespace IsLink.API.Models.DTOs;

// ============================================
// Request DTOs
// ============================================

public class CreateReviewRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }
}

public class AddSellerResponseRequest
{
    [Required]
    public string Response { get; set; } = string.Empty;
}

// ============================================
// Response DTOs
// ============================================

public class ReviewListResponse
{
    public bool Success { get; set; }
    public ReviewDataDto Data { get; set; } = new();
}

public class ReviewDataDto
{
    public List<ReviewDto> Reviews { get; set; } = new();
    public ReviewStatsDto Stats { get; set; } = new();
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? SellerResponse { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReviewBuyerDto Buyer { get; set; } = new();
    public ReviewGigDto? Gig { get; set; }
}

public class ReviewBuyerDto
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Country { get; set; }
}

public class ReviewGigDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class ReviewStatsDto
{
    public int Total { get; set; }
    public decimal Average { get; set; }
    public RatingDistributionDto Distribution { get; set; } = new();
}

public class RatingDistributionDto
{
    public int FiveStar { get; set; }
    public int FourStar { get; set; }
    public int ThreeStar { get; set; }
    public int TwoStar { get; set; }
    public int OneStar { get; set; }
}

