using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using IsLink.API.Models.MongoDB;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Concurrent;
using Google.GenAI;

namespace IsLink.API.Services;

public class LinkerAIService : ILinkerAIService
{
    private readonly IMongoCollection<ChatHistory> _chatHistories;
    private readonly ApplicationDbContext _context;
    private readonly IGigService _gigService;
    private readonly Client? _geminiClient;
    private readonly string _geminiApiKey;
    private const string MODEL_NAME = "gemini-2.0-flash";

    // Fallback store when MongoDB is not reachable (keeps LinkerAI usable on hosted demos)
    private static readonly ConcurrentDictionary<string, ChatHistory> InMemorySessions = new();

    public LinkerAIService(
        IMongoDatabase database,
        ApplicationDbContext context,
        IGigService gigService,
        IConfiguration configuration)
    {
        _chatHistories = database.GetCollection<ChatHistory>("linkerai_conversations");
        _context = context;
        _gigService = gigService;
        
        // Get Gemini API key - check both formats (appsettings.json and environment variable)
        _geminiApiKey = configuration["Gemini:ApiKey"] 
            ?? Environment.GetEnvironmentVariable("Gemini__ApiKey")
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
            ?? "";
        
        if (string.IsNullOrEmpty(_geminiApiKey) || _geminiApiKey == "your-gemini-api-key-here")
        {
            Console.WriteLine("⚠️ LinkerAI: Gemini API key not configured. Set GEMINI_API_KEY or Gemini__ApiKey environment variable.");
            _geminiClient = null;
        }
        else
        {
            try
            {
                var keyPreview = _geminiApiKey.Length > 10 ? _geminiApiKey.Substring(0, 10) + "..." : "***";
                Console.WriteLine($"✅ LinkerAI: Gemini API key loaded (starts with: {keyPreview})");
                
                // Initialize Google GenAI client with the official SDK
                // Set the API key in the environment for the SDK
                Environment.SetEnvironmentVariable("GEMINI_API_KEY", _geminiApiKey);
                _geminiClient = new Client(apiKey: _geminiApiKey);
                Console.WriteLine($"✅ LinkerAI: Using {MODEL_NAME} model");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LinkerAI: Failed to initialize Gemini client: {ex.Message}");
                _geminiClient = null;
            }
        }
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

        try
        {
            await _chatHistories.InsertOneAsync(chatHistory);
        }
        catch (Exception ex)
        {
            // MongoDB might be blocked/whitelisted on Atlas; don't break LinkerAI
            Console.WriteLine($"⚠️ LinkerAI MongoDB insert failed, using in-memory session store. Error: {ex.Message}");
            InMemorySessions[sessionId] = chatHistory;
        }

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

        ChatHistory? chatHistory = null;
        try
        {
            chatHistory = await _chatHistories
                .Find(c => c.SessionId == request.SessionId && c.IsActive)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ LinkerAI MongoDB find failed, trying in-memory store. Error: {ex.Message}");
            InMemorySessions.TryGetValue(request.SessionId, out chatHistory);
        }

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

        // Build conversation context for Gemini
        var systemMessage = chatHistory.Messages.FirstOrDefault(m => m.Role == "system");
        var systemPrompt = systemMessage?.Content ?? GetSystemPrompt();
        
        // Build the full prompt with context
        var conversationContext = new StringBuilder();
        conversationContext.AppendLine($"[System: {systemPrompt}]");
        conversationContext.AppendLine();
        
        // Add conversation history
        foreach (var msg in chatHistory.Messages.Where(m => m.Role != "system"))
        {
            var role = msg.Role == "user" ? "User" : "Assistant";
            conversationContext.AppendLine($"{role}: {msg.Content}");
        }

        string aiResponse;
        try
        {
            if (_geminiClient == null)
            {
                throw new Exception("Gemini API key not configured. Set GEMINI_API_KEY environment variable.");
            }
            
            Console.WriteLine($"🤖 LinkerAI: Calling Gemini API ({MODEL_NAME})...");
            
            // Use the official Google GenAI SDK
            var response = await _geminiClient.Models.GenerateContentAsync(
                model: MODEL_NAME,
                contents: conversationContext.ToString()
            );
            
            aiResponse = response?.Candidates?[0]?.Content?.Parts?[0]?.Text 
                ?? "I apologize, but I couldn't generate a response. Please try again.";
            
            Console.WriteLine($"✅ LinkerAI: Got response from Gemini");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ LinkerAI Gemini error: {ex.Message}");
            // Provide a helpful fallback response instead of crashing
            aiResponse = $"I'm having trouble connecting to my AI backend right now. Error: {ex.Message}. Please check that the Gemini API key is configured correctly on the server (environment variable: Gemini__ApiKey).";
        }

        // Add AI response to history
        chatHistory.Messages.Add(new ChatMessage
        {
            Role = "assistant",
            Content = aiResponse,
            Timestamp = DateTime.UtcNow
        });

        chatHistory.Metadata.LastActivityAt = DateTime.UtcNow;
        try
        {
            await _chatHistories.ReplaceOneAsync(c => c.Id == chatHistory.Id, chatHistory);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ LinkerAI MongoDB update failed, persisting to in-memory store. Error: {ex.Message}");
            InMemorySessions[chatHistory.SessionId] = chatHistory;
        }

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
        ChatHistory? chatHistory = null;
        try
        {
            chatHistory = await _chatHistories
                .Find(c => c.SessionId == sessionId && c.IsActive)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ LinkerAI MongoDB find failed in GetRecommendations, trying in-memory store. Error: {ex.Message}");
            InMemorySessions.TryGetValue(sessionId, out chatHistory);
        }

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
            score += (double)gig.Rating;

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
        return @"You are LinkerAI on IsLink freelance marketplace. Help users find services.

RULES:
- Ask ONE question at a time
- Get: project type, budget (USD), deadline, requirements
- Be brief and helpful
- When you have all info, say: ""Perfect! Let me find the best services for you...""";
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

