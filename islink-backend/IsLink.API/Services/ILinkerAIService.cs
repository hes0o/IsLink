using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface ILinkerAIService
{
    Task<LinkerAIChatResponse> StartConversationAsync(LinkerAIStartRequest request);
    Task<LinkerAIChatResponse> SendMessageAsync(LinkerAIChatRequest request);
    Task<LinkerAIRecommendations?> GetRecommendationsAsync(string sessionId);
    Task<LinkerAIChatResponse?> GetActiveSessionAsync(string userId);
    Task<List<ChatSessionDto>> GetUserSessionsAsync(string userId);
}

