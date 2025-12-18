using IsLink.API.Models.DTOs;
using IsLink.API.Services;

namespace IsLink.API.Tests;

public sealed class FakeLinkerAIService : ILinkerAIService
{
    private readonly Dictionary<string, List<string>> _sessions = new();

    public Task<LinkerAIChatResponse> StartConversationAsync(LinkerAIStartRequest request)
    {
        var sessionId = Guid.NewGuid().ToString("N");
        _sessions[sessionId] = new List<string>();

        var greeting = "Hello! I'm LinkerAI (test mode). Tell me what you're building and your budget/deadline.";
        if (!string.IsNullOrWhiteSpace(request.InitialMessage))
        {
            _sessions[sessionId].Add(request.InitialMessage);
        }

        return Task.FromResult(new LinkerAIChatResponse
        {
            Success = true,
            SessionId = sessionId,
            Message = greeting,
            IsComplete = false,
            MissingInfo = new List<string> { "budget", "deadline", "project type" }
        });
    }

    public Task<LinkerAIChatResponse> SendMessageAsync(LinkerAIChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId) || !_sessions.ContainsKey(request.SessionId))
        {
            throw new KeyNotFoundException("Session not found");
        }

        _sessions[request.SessionId].Add(request.Message);

        // Simple rule: if message mentions both a budget ($/usd/number) and a deadline (day/week/month),
        // mark complete and return mock recommendations.
        var text = request.Message.ToLowerInvariant();
        var hasBudget = text.Contains("$") || text.Contains("usd") || text.Any(char.IsDigit);
        var hasDeadline = text.Contains("day") || text.Contains("week") || text.Contains("month");

        if (hasBudget && hasDeadline)
        {
            return Task.FromResult(new LinkerAIChatResponse
            {
                Success = true,
                SessionId = request.SessionId!,
                Message = "Got it. Here are test recommendations.",
                IsComplete = true,
                Recommendations = new LinkerAIRecommendations
                {
                    ProjectSummary = new ProjectSummary
                    {
                        ProjectType = "website",
                        Description = "Test project",
                        KeyRequirements = new List<string> { "responsive", "fast", "secure" }
                    },
                    Budget = new BudgetBreakdown
                    {
                        TotalBudget = 500,
                        TotalCost = 450,
                        Remaining = 50,
                        ServiceCosts = new List<ServiceCost>
                        {
                            new() { ServiceName = "Service A", Cost = 300 },
                            new() { ServiceName = "Service B", Cost = 150 }
                        }
                    },
                    Timeline = new Timeline
                    {
                        TotalDays = 14,
                        DeadlineDays = 30,
                        IsFeasible = true,
                        Items = new List<TimelineItem>
                        {
                            new() { ServiceName = "Service A", Days = 10, StartDay = 1, EndDay = 10 },
                            new() { ServiceName = "Service B", Days = 4, StartDay = 11, EndDay = 14 }
                        }
                    },
                    Services = new List<RecommendedService>
                    {
                        new()
                        {
                            GigId = Guid.NewGuid(),
                            Title = "I will build your website (test)",
                            Slug = "test-website-gig",
                            Category = "Web Development",
                            SellerName = "Test Seller",
                            Price = 300,
                            DeliveryDays = 10,
                            Rating = 4.9m,
                            ReviewCount = 120,
                            ImageUrl = null,
                            Reason = "Matches your budget and deadline in test mode",
                            PackageType = "standard"
                        }
                    }
                }
            });
        }

        return Task.FromResult(new LinkerAIChatResponse
        {
            Success = true,
            SessionId = request.SessionId!,
            Message = "Test mode: please share budget and deadline (e.g. $500, 2 weeks).",
            IsComplete = false,
            MissingInfo = new List<string> { "budget", "deadline" }
        });
    }

    public Task<LinkerAIRecommendations?> GetRecommendationsAsync(string sessionId)
    {
        // In this fake, recommendations are returned as part of chat response.
        return Task.FromResult<LinkerAIRecommendations?>(null);
    }
}


