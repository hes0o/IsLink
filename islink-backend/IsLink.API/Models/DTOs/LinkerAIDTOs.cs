namespace IsLink.API.Models.DTOs;

// ============================================
// LinkerAI Request DTOs
// ============================================

public class LinkerAIChatRequest
{
    public string? SessionId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public class LinkerAIStartRequest
{
    public string? UserId { get; set; }
    public string? InitialMessage { get; set; }
}

// ============================================
// LinkerAI Response DTOs
// ============================================

public class LinkerAIChatResponse
{
    public bool Success { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsComplete { get; set; } // True when AI has enough info to recommend
    public LinkerAIRecommendations? Recommendations { get; set; }
    public List<string> MissingInfo { get; set; } = new(); // Questions AI wants to ask
}

public class LinkerAIRecommendations
{
    public ProjectSummary ProjectSummary { get; set; } = new();
    public List<RecommendedService> Services { get; set; } = new();
    public BudgetBreakdown Budget { get; set; } = new();
    public Timeline Timeline { get; set; } = new();
}

public class ProjectSummary
{
    public string ProjectType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? BrandName { get; set; }
    public List<string> KeyRequirements { get; set; } = new();
}

public class RecommendedService
{
    public Guid GigId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DeliveryDays { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? ImageUrl { get; set; }
    public string Reason { get; set; } = string.Empty; // Why this service was recommended
    public string PackageType { get; set; } = "basic"; // basic, standard, premium
}

public class BudgetBreakdown
{
    public decimal TotalBudget { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Remaining { get; set; }
    public List<ServiceCost> ServiceCosts { get; set; } = new();
}

public class ServiceCost
{
    public string ServiceName { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}

public class Timeline
{
    public int TotalDays { get; set; }
    public int DeadlineDays { get; set; }
    public bool IsFeasible { get; set; }
    public List<TimelineItem> Items { get; set; } = new();
}

public class TimelineItem
{
    public string ServiceName { get; set; } = string.Empty;
    public int Days { get; set; }
    public int StartDay { get; set; }
    public int EndDay { get; set; }
}

public class ChatSessionDto
{
    public string SessionId { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastActivityAt { get; set; }
}

