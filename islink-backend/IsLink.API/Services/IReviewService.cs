using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface IReviewService
{
    Task<ReviewListResponse> GetGigReviewsAsync(Guid gigId, int limit, int offset);
    Task<ReviewListResponse> GetSellerReviewsAsync(string username, int limit, int offset);
    Task<(bool Success, string Message, ReviewDto? Data)> CreateReviewAsync(Guid buyerId, CreateReviewRequest request);
    Task<(bool Success, string Message)> AddSellerResponseAsync(Guid sellerId, Guid reviewId, string response);
}

