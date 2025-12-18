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
using System.Net.Http.Headers;

namespace IsLink.API.Services;

public class LinkerAIService : ILinkerAIService
{
    private readonly IMongoCollection<ChatHistory> _chatHistories;
    private readonly ApplicationDbContext _context;
    private readonly IGigService _gigService;
    private readonly Client? _geminiClient;
    private readonly string _geminiApiKey;
    private const string MODEL_NAME = "gemini-2.0-flash";
    private readonly HttpClient _httpClient;
    private readonly string _groqApiKey;
    private const string GROQ_MODEL_NAME = "llama-3.1-70b-versatile";
    private readonly string _aiProviderMode;

    // Fallback store when MongoDB is not reachable (keeps LinkerAI usable on hosted demos)
    private static readonly ConcurrentDictionary<string, ChatHistory> InMemorySessions = new();

    public LinkerAIService(
        IMongoDatabase database,
        ApplicationDbContext context,
        IGigService gigService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _chatHistories = database.GetCollection<ChatHistory>("linkerai_conversations");
        _context = context;
        _gigService = gigService;
        _httpClient = httpClientFactory.CreateClient();
        _aiProviderMode = (Environment.GetEnvironmentVariable("AI_PROVIDER") ?? configuration["AI_PROVIDER"] ?? "auto").Trim().ToLowerInvariant();
        
        // Groq API key (preferred free-tier provider)
        _groqApiKey = configuration["Groq:ApiKey"]
            ?? Environment.GetEnvironmentVariable("Groq__ApiKey")
            ?? Environment.GetEnvironmentVariable("GROQ_API_KEY")
            ?? "";
        
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

    private sealed record AIResult(bool Success, string Text, string Provider, bool IsRateLimitedOrQuota, string? ErrorMessage);

    private async Task<AIResult> TryGetAiResponseAsync(string conversationContext)
    {
        // Decide provider order
        var order = _aiProviderMode switch
        {
            "groq" => new[] { "groq" },
            "gemini" => new[] { "gemini" },
            "fallback" => Array.Empty<string>(),
            _ => new[] { "groq", "gemini" } // auto
        };

        foreach (var provider in order)
        {
            if (provider == "groq")
            {
                var groq = await TryGroqAsync(conversationContext);
                if (groq.Success || groq.IsRateLimitedOrQuota == false)
                {
                    // Success OR a non-rate-limit failure (return it to caller)
                    return groq;
                }

                // Rate-limited: try next provider
                continue;
            }

            if (provider == "gemini")
            {
                var gemini = await TryGeminiAsync(conversationContext);
                if (gemini.Success || gemini.IsRateLimitedOrQuota == false)
                {
                    return gemini;
                }
                continue;
            }
        }

        return new AIResult(false, "", "fallback", true, "All AI providers unavailable");
    }

    private async Task<AIResult> TryGeminiAsync(string conversationContext)
    {
        try
        {
            if (_geminiClient == null)
            {
                return new AIResult(false, "", "gemini", false, "Gemini API key not configured");
            }

            Console.WriteLine($"🤖 LinkerAI: Calling Gemini API ({MODEL_NAME})...");

            var response = await _geminiClient.Models.GenerateContentAsync(
                model: MODEL_NAME,
                contents: conversationContext
            );

            var text = response?.Candidates?[0]?.Content?.Parts?[0]?.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return new AIResult(false, "", "gemini", false, "Empty Gemini response");
            }

            Console.WriteLine("✅ LinkerAI: Got response from Gemini");
            return new AIResult(true, text, "gemini", false, null);
        }
        catch (Exception ex)
        {
            var msg = ex.Message ?? "Gemini error";
            Console.WriteLine($"❌ LinkerAI Gemini error: {msg}");
            var isQuota =
                msg.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("rate", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("429", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("RESOURCE_EXHAUSTED", StringComparison.OrdinalIgnoreCase);
            return new AIResult(false, "", "gemini", isQuota, msg);
        }
    }

    private async Task<AIResult> TryGroqAsync(string conversationContext)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_groqApiKey))
            {
                return new AIResult(false, "", "groq", false, "Groq API key not configured");
            }

            Console.WriteLine($"🤖 LinkerAI: Calling Groq API ({GROQ_MODEL_NAME})...");

            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _groqApiKey);

            var payload = new
            {
                model = GROQ_MODEL_NAME,
                messages = new[]
                {
                    new { role = "user", content = conversationContext }
                },
                temperature = 0.4,
                max_tokens = 500
            };

            req.Content = JsonContent.Create(payload);
            var resp = await _httpClient.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                var isRate = (int)resp.StatusCode == 429 || body.Contains("rate", StringComparison.OrdinalIgnoreCase) || body.Contains("quota", StringComparison.OrdinalIgnoreCase);
                return new AIResult(false, "", "groq", isRate, $"Groq HTTP {(int)resp.StatusCode}: {body}");
            }

            using var doc = JsonDocument.Parse(body);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
            {
                return new AIResult(false, "", "groq", false, "Empty Groq response");
            }

            Console.WriteLine("✅ LinkerAI: Got response from Groq");
            return new AIResult(true, content, "groq", false, null);
        }
        catch (Exception ex)
        {
            var msg = ex.Message ?? "Groq error";
            Console.WriteLine($"❌ LinkerAI Groq error: {msg}");
            var isRate =
                msg.Contains("rate", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("429", StringComparison.OrdinalIgnoreCase);
            return new AIResult(false, "", "groq", isRate, msg);
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

        var aiResult = await TryGetAiResponseAsync(conversationContext.ToString());
        var aiResponse = aiResult.Text;
        var aiHadError = !aiResult.Success;
        var aiErrorMessage = aiResult.ErrorMessage ?? string.Empty;

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

        // If Gemini is down/quota-limited, fall back to deterministic requirement extraction + marketplace matching.
        // This makes LinkerAI usable on free tiers.
        var isQuotaOrRateLimit = aiHadError && aiResult.IsRateLimitedOrQuota;

        var requirements = ExtractRequirements(chatHistory.Messages);
        var missingInfo = new List<string>();
        if (string.IsNullOrWhiteSpace(requirements.ProjectType)) missingInfo.Add("project type");
        if (!requirements.Budget.HasValue) missingInfo.Add("budget");
        if (!requirements.DeadlineDays.HasValue) missingInfo.Add("deadline");

        var hasMinimumInfo = missingInfo.Count == 0;

        // Check if AI indicates it has enough information (normal path)
        var isComplete = !aiHadError && CheckIfComplete(aiResponse, chatHistory.Messages);

        LinkerAIRecommendations? recommendations = null;
        if (isComplete || (isQuotaOrRateLimit && hasMinimumInfo))
        {
            recommendations = await GenerateRecommendationsAsync(chatHistory);
            isComplete = true;
        }

        if (aiHadError)
        {
            if (isQuotaOrRateLimit)
            {
                // Friendly response when quota is hit; keep the conversation moving.
                aiResponse = hasMinimumInfo
                    ? $"AI provider ({aiResult.Provider}) is currently rate-limited/quota-exceeded. I can still recommend services using marketplace matching. Here are results based on your budget/deadline."
                    : $"AI provider ({aiResult.Provider}) is currently rate-limited/quota-exceeded. No problem — I can still help without it. Please tell me your {string.Join(" and ", missingInfo)}.";
            }
            else
            {
                // Non-quota errors: still return a clear failure so frontend can show it properly.
                return new LinkerAIChatResponse
                {
                    Success = false,
                    SessionId = request.SessionId,
                    Message = $"LinkerAI backend error ({aiResult.Provider}): {aiErrorMessage}",
                    IsComplete = false,
                    MissingInfo = missingInfo
                };
            }
        }

        return new LinkerAIChatResponse
        {
            Success = true,
            SessionId = request.SessionId,
            Message = aiResponse,
            IsComplete = isComplete,
            Recommendations = recommendations,
            MissingInfo = missingInfo
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
        // Prefer explicit currency formats first: "$500", "$ 500", "500$"
        var match = Regex.Match(text, @"\$\s*([0-9]+(?:\.[0-9]+)?)");
        if (match.Success && decimal.TryParse(match.Groups[1].Value, out var budget1))
        {
            return budget1;
        }

        match = Regex.Match(text, @"([0-9]+(?:\.[0-9]+)?)\s*\$");
        if (match.Success && decimal.TryParse(match.Groups[1].Value, out var budget2))
        {
            return budget2;
        }

        // Then try keyword-based: "budget is 500", "max 500", "up to 500 USD"
        match = Regex.Match(
            text,
            @"(?i)\b(?:budget|max|up to|not more than|no more than)\b[^\d]{0,12}([0-9]+(?:\.[0-9]+)?)\s*(?:usd|dollars?)?\b");
        if (match.Success && decimal.TryParse(match.Groups[1].Value, out var budget3))
        {
            return budget3;
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

