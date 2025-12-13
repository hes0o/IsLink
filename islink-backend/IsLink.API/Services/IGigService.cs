using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface IGigService
{
    Task<GigListResponse> GetGigsAsync(GigFilterRequest filter);
    Task<GigDetailResponse> GetGigBySlugAsync(string slug);
    Task<GigDetailResponse> CreateGigAsync(Guid sellerId, CreateGigRequest request);
    Task<GigListResponse> GetGigsBySellerAsync(string username);
}

