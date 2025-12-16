using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using IsLink.API.Models.MongoDB;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IsLink.API.Services;

public class LinkerAIService : ILinkerAIService
{
    private readonly IMongoCollection<ChatHistory> _chatHistories;
    private readonly ApplicationDbContext _context;
    private readonly IGigService _gigService;
    private readonly OpenAIClient _openAIClient;
    private readonly string _openAIKey;

    public LinkerAIService(
        IMongoDatabase database,
        ApplicationDbContext context,
        IGigService gigService,
        IConfiguration configuration)
    {
        _chatHistories = database.GetCollection<ChatHistory>("linkerai_conversations");
        _context = context;
        _gigService = gigService;
        _openAIKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured");
        _openAIClient = new OpenAIClient(_openAIKey);
    }

    public async Task<LinkerAIChatResponse> StartConversationAsync(LinkerAIStartRequest request)
    {
        var sessionId = Guid.NewGuid().ToString();

        var systemPrompt = GetSystemPrompt();

        var chatHistory = new ChatHistory
        {
            SessionId = sessionId,
            UserId = request.UserId,
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "system",
                    Content = systemPrompt,
                    Timestamp = DateTime.UtcNow
                }
            },
            Metadata = new ChatMetadata
            {
                StartedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            }
        };

        if (!string.IsNullOrEmpty(request.InitialMessage))
        {
            chatHistory.Messages.Add(new ChatMessage
            {
                Role = "user",
                Content = request.InitialMessage,
                Timestamp = DateTime.UtcNow
            });
        }

        await _chatHistories.InsertOneAsync(chatHistory);

        if (string.IsNullOrEmpty(request.InitialMessage))
        {
            // Start with a greeting
            return new LinkerAIChatResponse
            {
                Success = true,
                SessionId = sessionId,
                Message = "Hello! I'm LinkerAI, your project assistant. I'll help you find the perfect services on IsLink for your project. Let's start! What kind of project are you looking to build?",
                IsComplete = false
            };
        }

        return await SendMessageAsync(new LinkerAIChatRequest
        {
            SessionId = sessionId,
            Message = request.InitialMessage,
            UserId = request.UserId
        });
    }

    public async Task<LinkerAIChatResponse> SendMessageAsync(LinkerAIChatRequest request)
    {
        if (string.IsNullOrEmpty(request.SessionId))
        {
            throw new ArgumentException("SessionId is required");
        }

        var chatHistory = await _chatHistories
            .Find(c => c.SessionId == request.SessionId && c.IsActive)
            .FirstOrDefaultAsync();

        if (chatHistory == null)
        {
            throw new KeyNotFoundException("Chat session not found");
        }

        // Add user message
        chatHistory.Messages.Add(new ChatMessage
        {
            Role = "user",
            Content = request.Message,
            Timestamp = DateTime.UtcNow
        });

        // Convert to OpenAI format
        var messages = chatHistory.Messages.Select(m => new ChatMessage
        {
            Role = m.Role switch
            {
                "system" => ChatMessageRole.System,
                "user" => ChatMessageRole.User,
                "assistant" => ChatMessageRole.Assistant,
                _ => ChatMessageRole.User
            },
            Content = m.Content
        }).ToList();

        // Call OpenAI
        var chatCompletionOptions = new ChatCompletionOptions
        {
            Messages = messages,
            Model = "gpt-3.5-turbo",
            Temperature = 0.7f,
            MaxTokens = 1000
        };

        var completion = await _openAIClient.ChatEndpoint.GetCompletionAsync(chatCompletionOptions);
        var aiResponse = completion.FirstChoice.Message.Content;

        // Add AI response to history
        chatHistory.Messages.Add(new ChatMessage
        {
            Role = "assistant",
            Content = aiResponse,
            Timestamp = DateTime.UtcNow
        });

        chatHistory.Metadata.LastActivityAt = DateTime.UtcNow;
        await _chatHistories.ReplaceOneAsync(c => c.Id == chatHistory.Id, chatHistory);

        // Check if AI indicates it has enough information
        var isComplete = CheckIfComplete(aiResponse, chatHistory.Messages);

        LinkerAIRecommendations? recommendations = null;
        if (isComplete)
        {
            recommendations = await GenerateRecommendationsAsync(chatHistory);
        }

        return new LinkerAIChatResponse
        {
            Success = true,
            SessionId = request.SessionId,
            Message = aiResponse,
            IsComplete = isComplete,
            Recommendations = recommendations
        };
    }

    public async Task<LinkerAIRecommendations?> GetRecommendationsAsync(string sessionId)
    {
        var chatHistory = await _chatHistories
            .Find(c => c.SessionId == sessionId && c.IsActive)
            .FirstOrDefaultAsync();

        if (chatHistory == null)
        {
            return null;
        }

        return await GenerateRecommendationsAsync(chatHistory);
    }

    private async Task<LinkerAIRecommendations> GenerateRecommendationsAsync(ChatHistory chatHistory)
    {
        // Extract requirements from conversation
        var requirements = ExtractRequirements(chatHistory.Messages);

        // Get all available gigs
        var allGigs = await _gigService.GetGigsAsync(new GigFilterRequest
        {
            Limit = 100,
            SortBy = "rating"
        });

        // Filter and match gigs based on requirements
        var matchedGigs = MatchGigsToRequirements(allGigs.Data, requirements);

        // Generate project summary
        var projectSummary = new ProjectSummary
        {
            ProjectType = requirements.ProjectType ?? "General Project",
            Description = requirements.Description ?? string.Join(" ", chatHistory.Messages.Where(m => m.Role == "user").Select(m => m.Content)),
            BrandName = requirements.BrandName,
            KeyRequirements = ExtractKeyRequirements(chatHistory.Messages)
        };

        // Calculate budget breakdown
        var totalCost = matchedGigs.Sum(g => g.Price);
        var budgetBreakdown = new BudgetBreakdown
        {
            TotalBudget = requirements.Budget ?? 0,
            TotalCost = totalCost,
            Remaining = (requirements.Budget ?? 0) - totalCost,
            ServiceCosts = matchedGigs.Select(g => new ServiceCost
            {
                ServiceName = g.Title,
                Cost = g.Price
            }).ToList()
        };

        // Calculate timeline
        var maxDeliveryDays = matchedGigs.Any() ? matchedGigs.Max(g => g.DeliveryDays) : 0;
        var timeline = new Timeline
        {
            TotalDays = maxDeliveryDays,
            DeadlineDays = requirements.DeadlineDays ?? 0,
            IsFeasible = requirements.DeadlineDays == null || maxDeliveryDays <= requirements.DeadlineDays,
            Items = matchedGigs.Select((g, index) => new TimelineItem
            {
                ServiceName = g.Title,
                Days = g.DeliveryDays,
                StartDay = 0,
                EndDay = g.DeliveryDays
            }).ToList()
        };

        return new LinkerAIRecommendations
        {
            ProjectSummary = projectSummary,
            Services = matchedGigs.Take(10).ToList(), // Top 10 matches
            Budget = budgetBreakdown,
            Timeline = timeline
        };
    }

    private List<RecommendedService> MatchGigsToRequirements(List<GigSummaryDto> gigs, ExtractedRequirements requirements)
    {
        var matched = new List<RecommendedService>();

        foreach (var gig in gigs)
        {
            var score = 0.0;
            var reasons = new List<string>();

            // Category matching
            if (!string.IsNullOrEmpty(requirements.ProjectType))
            {
                var projectTypeLower = requirements.ProjectType.ToLower();
                if (gig.Category?.Name?.ToLower().Contains(projectTypeLower) == true ||
                    gig.Title.ToLower().Contains(projectTypeLower))
                {
                    score += 10;
                    reasons.Add("Matches project type");
                }
            }

            // Budget matching
            if (requirements.Budget.HasValue && gig.Packages?.Basic != null)
            {
                if (gig.Packages.Basic.Price <= requirements.Budget.Value)
                {
                    score += 5;
                    reasons.Add("Within budget");
                }
                else
                {
                    score -= 5; // Penalty for over budget
                }
            }

            // Deadline matching
            if (requirements.DeadlineDays.HasValue && gig.Packages?.Basic != null)
            {
                if (gig.Packages.Basic.DeliveryDays <= requirements.DeadlineDays.Value)
                {
                    score += 5;
                    reasons.Add("Meets deadline");
                }
            }

            // Rating boost
            score += gig.Rating;

            if (score > 0)
            {
                matched.Add(new RecommendedService
                {
                    GigId = gig.Id,
                    Title = gig.Title,
                    Slug = gig.Slug,
                    Category = gig.Category?.Name ?? "Uncategorized",
                    SellerName = gig.Seller?.Username ?? "Unknown",
                    Price = gig.Packages?.Basic?.Price ?? 0,
                    DeliveryDays = gig.Packages?.Basic?.DeliveryDays ?? 0,
                    Rating = gig.Rating,
                    ReviewCount = gig.ReviewCount,
                    ImageUrl = gig.Images?.FirstOrDefault(),
                    Reason = string.Join(", ", reasons),
                    PackageType = "basic"
                });
            }
        }

        return matched.OrderByDescending(m => m.Rating).ThenBy(m => m.Price).ToList();
    }

    private ExtractedRequirements ExtractRequirements(List<ChatMessage> messages)
    {
        var conversationText = string.Join(" ", messages.Where(m => m.Role == "user").Select(m => m.Content));

        return new ExtractedRequirements
        {
            ProjectType = ExtractProjectType(conversationText),
            Budget = ExtractBudget(conversationText),
            DeadlineDays = ExtractDeadline(conversationText),
            Description = conversationText,
            BrandName = ExtractBrandName(conversationText)
        };
    }

    private string? ExtractProjectType(string text)
    {
        var lowerText = text.ToLower();
        if (lowerText.Contains("website") || lowerText.Contains("web")) return "Website";
        if (lowerText.Contains("e-commerce") || lowerText.Contains("ecommerce") || lowerText.Contains("online store")) return "E-commerce";
        if (lowerText.Contains("logo")) return "Logo Design";
        if (lowerText.Contains("app") || lowerText.Contains("application")) return "Mobile App";
        if (lowerText.Contains("seo")) return "SEO";
        if (lowerText.Contains("marketing")) return "Digital Marketing";
        return null;
    }

    private decimal? ExtractBudget(string text)
    {
        var match = Regex.Match(text, @"\$?(\d+)\s*(?:dollars?|USD|usd|\$|budget)");
        if (match.Success && decimal.TryParse(match.Groups[1].Value, out var budget))
        {
            return budget;
        }
        return null;
    }

    private int? ExtractDeadline(string text)
    {
        var match = Regex.Match(text, @"(\d+)\s*(?:days?|weeks?|months?)", RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Groups[1].Value, out var days))
        {
            if (text.ToLower().Contains("week"))
            {
                return days * 7;
            }
            if (text.ToLower().Contains("month"))
            {
                return days * 30;
            }
            return days;
        }
        return null;
    }

    private string? ExtractBrandName(string text)
    {
        // Simple extraction - could be enhanced
        var match = Regex.Match(text, @"(?:brand|company|name)[\s:]+([A-Z][a-zA-Z\s]+)", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private List<string> ExtractKeyRequirements(List<ChatMessage> messages)
    {
        var requirements = new List<string>();
        var userMessages = string.Join(" ", messages.Where(m => m.Role == "user").Select(m => m.Content));
        
        // Simple extraction - could be enhanced with AI
        if (userMessages.Contains("responsive")) requirements.Add("Responsive Design");
        if (userMessages.Contains("mobile")) requirements.Add("Mobile Friendly");
        if (userMessages.Contains("payment")) requirements.Add("Payment Integration");
        if (userMessages.Contains("seo")) requirements.Add("SEO Optimized");
        
        return requirements;
    }

    private bool CheckIfComplete(string aiResponse, List<ChatMessage> messages)
    {
        var lowerResponse = aiResponse.ToLower();
        var indicators = new[] { "here are", "recommendations", "i recommend", "suggest", "here's what" };
        return indicators.Any(indicator => lowerResponse.Contains(indicator)) && messages.Count(m => m.Role == "user") >= 3;
    }

    private string GetSystemPrompt()
    {
        return @"You are LinkerAI, an intelligent project assistant on IsLink, a freelancing marketplace. Your role is to help customers find the perfect services for their projects.

IMPORTANT GUIDELINES:
1. Be friendly, conversational, and professional
2. Ask ONE question at a time - don't overwhelm the user
3. Extract these key pieces of information:
   - Project type (e.g., e-commerce website, mobile app, logo design)
   - Budget (maximum amount they can spend in USD)
   - Deadline (number of days or weeks they have)
   - Brand/Company name (if applicable)
   - Specific requirements and features needed

4. Ask follow-up questions if any information is missing:
   - If no budget mentioned: ""What's your budget for this project?""
   - If no deadline mentioned: ""When do you need this completed?""
   - If no brand name: ""What's your brand or company name?""
   - If requirements are vague: Ask for more specific details

5. Once you have collected: project type, budget, deadline, and main requirements (usually after 3-5 exchanges), respond with:
   ""Perfect! I have all the information I need. Based on your requirements, I'll now find the best services for you. Let me search our marketplace...""

Remember: Focus on understanding their needs completely before recommending services. Be thorough but efficient.";
    }

    private class ExtractedRequirements
    {
        public string? ProjectType { get; set; }
        public decimal? Budget { get; set; }
        public int? DeadlineDays { get; set; }
        public string? Description { get; set; }
        public string? BrandName { get; set; }
    }
}

