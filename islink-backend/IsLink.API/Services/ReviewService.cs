using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ReviewService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<ReviewListResponse> GetGigReviewsAsync(Guid gigId, int limit, int offset)
    {
        var reviews = await _context.Reviews
            .Include(r => r.Buyer)
            .Where(r => r.GigId == gigId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var stats = await GetReviewStatsAsync(gigId);

        return new ReviewListResponse
        {
            Success = true,
            Data = new ReviewDataDto
            {
                Reviews = reviews.Select(MapToReviewDto).ToList(),
                Stats = stats
            }
        };
    }

    public async Task<ReviewListResponse> GetSellerReviewsAsync(string username, int limit, int offset)
    {
        var seller = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        if (seller == null)
        {
            return new ReviewListResponse { Success = false };
        }

        var reviews = await _context.Reviews
            .Include(r => r.Buyer)
            .Include(r => r.Gig)
            .Where(r => r.SellerId == seller.Id)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return new ReviewListResponse
        {
            Success = true,
            Data = new ReviewDataDto
            {
                Reviews = reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    SellerResponse = r.SellerResponse,
                    CreatedAt = r.CreatedAt,
                    Buyer = r.Buyer == null ? new() : new ReviewBuyerDto
                    {
                        Username = r.Buyer.Username,
                        FullName = r.Buyer.FullName,
                        Avatar = r.Buyer.AvatarUrl,
                        Country = r.Buyer.Country
                    },
                    Gig = r.Gig == null ? null : new ReviewGigDto
                    {
                        Title = r.Gig.Title,
                        Slug = r.Gig.Slug
                    }
                }).ToList()
            }
        };
    }

    public async Task<(bool Success, string Message, ReviewDto? Data)> CreateReviewAsync(Guid buyerId, CreateReviewRequest request)
    {
        var order = await _context.Orders
            .Include(o => o.Gig)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.BuyerId == buyerId && o.Status == "completed");

        if (order == null)
        {
            return (false, "Order not found or not eligible for review", null);
        }

        var existingReview = await _context.Reviews.AnyAsync(r => r.OrderId == request.OrderId);
        if (existingReview)
        {
            return (false, "Review already exists for this order", null);
        }

        var review = new Review
        {
            OrderId = request.OrderId,
            GigId = order.GigId,
            BuyerId = buyerId,
            SellerId = order.SellerId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Update gig and seller ratings (triggers would do this in PostgreSQL)
        await UpdateRatingsAsync(order.GigId, order.SellerId);

        // Send notification
        if (order.SellerId.HasValue)
        {
            await _notificationService.CreateNotificationAsync(
                order.SellerId.Value.ToString(),
                "new_review",
                "New Review Received! ⭐",
                $"You received a {request.Rating}-star review",
                new Dictionary<string, object> { { "reviewId", review.Id }, { "rating", request.Rating } }
            );
        }

        return (true, "Review submitted successfully", new ReviewDto { Id = review.Id, Rating = review.Rating });
    }

    public async Task<(bool Success, string Message)> AddSellerResponseAsync(Guid sellerId, Guid reviewId, string response)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.SellerId == sellerId && r.SellerResponse == null);

        if (review == null)
        {
            return (false, "Review not found or response already exists");
        }

        review.SellerResponse = response;
        await _context.SaveChangesAsync();

        return (true, "Response added successfully");
    }

    private async Task<ReviewStatsDto> GetReviewStatsAsync(Guid? gigId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.GigId == gigId)
            .Select(r => r.Rating)
            .ToListAsync();

        if (!reviews.Any())
        {
            return new ReviewStatsDto();
        }

        return new ReviewStatsDto
        {
            Total = reviews.Count,
            Average = (decimal)reviews.Average(),
            Distribution = new RatingDistributionDto
            {
                FiveStar = reviews.Count(r => r == 5),
                FourStar = reviews.Count(r => r == 4),
                ThreeStar = reviews.Count(r => r == 3),
                TwoStar = reviews.Count(r => r == 2),
                OneStar = reviews.Count(r => r == 1)
            }
        };
    }

    private async Task UpdateRatingsAsync(Guid? gigId, Guid? sellerId)
    {
        if (gigId.HasValue)
        {
            var gigReviews = await _context.Reviews.Where(r => r.GigId == gigId).ToListAsync();
            var gig = await _context.Gigs.FindAsync(gigId);
            if (gig != null && gigReviews.Any())
            {
                gig.Rating = (decimal)gigReviews.Average(r => r.Rating);
                gig.ReviewCount = gigReviews.Count;
            }
        }

        if (sellerId.HasValue)
        {
            var sellerReviews = await _context.Reviews.Where(r => r.SellerId == sellerId).ToListAsync();
            var seller = await _context.Users.FindAsync(sellerId);
            if (seller != null && sellerReviews.Any())
            {
                seller.Rating = (decimal)sellerReviews.Average(r => r.Rating);
                seller.ReviewCount = sellerReviews.Count;
            }
        }

        await _context.SaveChangesAsync();
    }

    private static ReviewDto MapToReviewDto(Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            SellerResponse = review.SellerResponse,
            CreatedAt = review.CreatedAt,
            Buyer = review.Buyer == null ? new() : new ReviewBuyerDto
            {
                Username = review.Buyer.Username,
                FullName = review.Buyer.FullName,
                Avatar = review.Buyer.AvatarUrl,
                Country = review.Buyer.Country
            }
        };
    }
}

