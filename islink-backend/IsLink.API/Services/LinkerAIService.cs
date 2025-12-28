using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Http.Headers;

namespace IsLink.API.Services;

public class LinkerAIService : ILinkerAIService
{
    private readonly ApplicationDbContext _context;
    private readonly IGigService _gigService;
    private readonly HttpClient _httpClient;
    private readonly string _groqApiKey;
    private const string GROQ_MODEL_NAME = "llama-3.3-70b-versatile";

    public LinkerAIService(
        ApplicationDbContext context,
        IGigService gigService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _context = context;
        _gigService = gigService;
        _httpClient = httpClientFactory.CreateClient();
        
        // Groq API key
        _groqApiKey = configuration["Groq:ApiKey"]
            ?? Environment.GetEnvironmentVariable("Groq__ApiKey")
            ?? Environment.GetEnvironmentVariable("GROQ_API_KEY")
            ?? "";
        
        if (string.IsNullOrWhiteSpace(_groqApiKey))
        {
            Console.WriteLine("⚠️ LinkerAI: Groq API key not configured. Set GROQ_API_KEY or Groq:ApiKey in appsettings.json");
        }
        else
        {
            Console.WriteLine("✅ LinkerAI: Groq API key loaded");
        }
    }

    private sealed record AIResult(bool Success, string Text, string? ErrorMessage);

    private async Task<AIResult> TryGetAiResponseAsync(string conversationContext)
    {
        return await TryGroqAsync(conversationContext);
    }

    private async Task<AIResult> TryGroqAsync(string conversationContext)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_groqApiKey))
            {
                return new AIResult(false, "", "Groq API key not configured. Please configure Groq:ApiKey in appsettings.json");
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
                return new AIResult(false, "", $"Groq API error (HTTP {(int)resp.StatusCode}): {body}");
            }

            using var doc = JsonDocument.Parse(body);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
            {
                return new AIResult(false, "", "Empty response from Groq API");
            }

            Console.WriteLine("✅ LinkerAI: Got response from Groq");
            return new AIResult(true, content, null);
        }
        catch (Exception ex)
        {
            var msg = ex.Message ?? "Groq API error";
            Console.WriteLine($"❌ LinkerAI Groq error: {msg}");
            return new AIResult(false, "", $"Groq API error: {msg}");
        }
    }

    public async Task<LinkerAIChatResponse> StartConversationAsync(LinkerAIStartRequest request)
    {
        var sessionId = Guid.NewGuid().ToString();

        var systemPrompt = GetSystemPrompt();

        var chatSession = new ChatSession
        {
            SessionId = sessionId,
            UserId = request.UserId ?? "guest", // Default to guest if null
            StartedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow,
            IsActive = true
        };

        // Add System Message
        chatSession.Messages.Add(new ChatMessage
        {
            Role = "system",
            Content = systemPrompt,
            Timestamp = DateTime.UtcNow
        });

        if (!string.IsNullOrEmpty(request.InitialMessage))
        {
            chatSession.Messages.Add(new ChatMessage
            {
                Role = "user",
                Content = request.InitialMessage,
                Timestamp = DateTime.UtcNow
            });
        }

        try
        {
            _context.ChatSessions.Add(chatSession);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ LinkerAI Database insert failed: {ex.Message}");
            throw; // Fail if DB fails, as we rely on persistence now
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

        // Retrieve session with messages
        var chatSession = await _context.ChatSessions
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.SessionId == request.SessionId && c.IsActive);

        if (chatSession == null)
        {
            throw new KeyNotFoundException("Chat session not found");
        }

        // Add user message
        var userMsg = new ChatMessage
        {
            Role = "user",
            Content = request.Message,
            Timestamp = DateTime.UtcNow,
            ChatSessionId = chatSession.Id
        };
        chatSession.Messages.Add(userMsg);
        _context.ChatMessages.Add(userMsg); // Explicit add to ensure tracking

        // 3. Auto-Generate Title if this is the first user message
        if (chatSession.Messages.Count(m => m.Role == "user") == 1 && (string.IsNullOrEmpty(chatSession.Title) || chatSession.Title == "New Chat"))
        {
            try 
            {
                var titlePrompt = $"Summarize the following user request into a very short title (max 5 words). Return ONLY the title text, no quotes, no prefixes.\n\nUser Request: {request.Message}";
                var titleResult = await TryGroqAsync(titlePrompt); // Reusing existing helper
                if (titleResult.Success)
                {
                    chatSession.Title = titleResult.Text.Trim().Trim('"').Trim('.');
                    // We don't save yet, it will be saved with the AI response below
                }
            } 
            catch (Exception ex) 
            {
                Console.WriteLine($"⚠️ Failed to generate title: {ex.Message}");
                // Ignore title failure, proceed with chat
            }
        }

        // Build conversation context for Gemini
        var systemMessage = chatSession.Messages.FirstOrDefault(m => m.Role == "system");
        var systemPrompt = systemMessage?.Content ?? GetSystemPrompt();
        
        // Build the full prompt with context
        var conversationContext = new StringBuilder();
        conversationContext.AppendLine($"[System: {systemPrompt}]");
        conversationContext.AppendLine();
        
        // Add conversation history
        foreach (var msg in chatSession.Messages.Where(m => m.Role != "system"))
        {
            var role = msg.Role == "user" ? "User" : "Assistant";
            conversationContext.AppendLine($"{role}: {msg.Content}");
        }

        var aiResult = await TryGetAiResponseAsync(conversationContext.ToString());
        
        if (!aiResult.Success)
        {
            return new LinkerAIChatResponse
            {
                Success = false,
                SessionId = request.SessionId,
                Message = $"Groq API error: {aiResult.ErrorMessage}",
                IsComplete = false
            };
        }

        var aiResponse = aiResult.Text;

        // Add AI response to history
        var aiMsg = new ChatMessage
        {
            Role = "assistant",
            Content = aiResponse,
            Timestamp = DateTime.UtcNow,
            ChatSessionId = chatSession.Id
        };
        chatSession.Messages.Add(aiMsg);
        _context.ChatMessages.Add(aiMsg);

        chatSession.LastActivityAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ LinkerAI Database update failed: {ex.Message}");
            throw;
        }

        var requirements = ExtractRequirements(chatSession.Messages);
        var missingInfo = new List<string>();
        if (string.IsNullOrWhiteSpace(requirements.ProjectType)) missingInfo.Add("project type");
        if (!requirements.Budget.HasValue) missingInfo.Add("budget");
        if (!requirements.DeadlineDays.HasValue) missingInfo.Add("deadline");

        // Check if AI indicates it has enough information
        var isComplete = CheckIfComplete(aiResponse, chatSession.Messages);

        LinkerAIRecommendations? recommendations = null;
        if (isComplete)
        {
            recommendations = await GenerateRecommendationsAsync(chatSession);
            isComplete = true;
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
        ChatSession? chatSession = null;
        try
        {
            chatSession = await _context.ChatSessions
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ LinkerAI Database find failed in GetRecommendations: {ex.Message}");
        }

        if (chatSession == null)
        {
            return null;
        }

        return await GenerateRecommendationsAsync(chatSession);
    }

    public async Task<LinkerAIChatResponse?> GetActiveSessionAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return null;

        // Find the most recent active session for this user
        ChatSession? chatSession = null;
        try
        {
            chatSession = await _context.ChatSessions
                .Include(c => c.Messages)
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.LastActivityAt)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ LinkerAI Database find failed in GetActiveSession: {ex.Message}");
            return null;
        }

        if (chatSession == null) return null;

        // Check for recommendations
        LinkerAIRecommendations? recommendations = null;
        var lastMsg = chatSession.Messages.LastOrDefault(m => m.Role == "assistant");
        
        // Safety check if last message exists
        if (lastMsg != null && CheckIfComplete(lastMsg.Content, chatSession.Messages))
        {
            recommendations = await GenerateRecommendationsAsync(chatSession);
        }

        return new LinkerAIChatResponse
        {
            Success = true,
            SessionId = chatSession.SessionId,
            Message = chatSession.Messages.LastOrDefault(m => m.Role == "assistant")?.Content ?? "",
            IsComplete = recommendations != null,
            Recommendations = recommendations,
        };
    }

    public async Task<ChatSessionDetailDto?> GetSessionAsync(string sessionId, string userId)
    {
        try
        {
            var session = await _context.ChatSessions
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.UserId == userId && c.IsActive);

            if (session == null) return null;

            // Check for recommendations
            LinkerAIRecommendations? recommendations = null;
            var lastMsg = session.Messages.LastOrDefault(m => m.Role == "assistant");
            if (lastMsg != null && CheckIfComplete(lastMsg.Content, session.Messages))
            {
                recommendations = await GenerateRecommendationsAsync(session);
            }

            return new ChatSessionDetailDto
            {
                SessionId = session.SessionId,
                LastActivityAt = session.LastActivityAt,
                Messages = session.Messages
                    .Where(m => m.Role != "system")
                    .OrderBy(m => m.Timestamp)
                    .Select(m => new ChatMessageDto
                    {
                        Role = m.Role,
                        Content = m.Content,
                        Timestamp = m.Timestamp
                    }).ToList(),
                Recommendations = recommendations,
                IsComplete = recommendations != null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Database fetch failed in GetSessionAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ChatSessionDto>> GetUserSessionsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return new List<ChatSessionDto>();

        try
        {
            var sessions = await _context.ChatSessions
                .Include(c => c.Messages)
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.LastActivityAt)
                .Take(20)
                .ToListAsync();

            return sessions.Select(s => new ChatSessionDto
            {
                SessionId = s.SessionId,
                Title = s.Title, // Map Title
                LastMessage = s.Messages.LastOrDefault()?.Content?.Take(50).ToString() ?? "New Conversation",
                LastActivityAt = s.LastActivityAt
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Database fetch failed in GetUserSessions: {ex.Message}");
            return new List<ChatSessionDto>();
        }
    }

    private async Task<LinkerAIRecommendations> GenerateRecommendationsAsync(ChatSession chatSession)
    {
        // Extract requirements from conversation
        var requirements = ExtractRequirements(chatSession.Messages);

        // Get all available gigs
        var allGigs = await _gigService.GetGigsAsync(new GigFilterRequest
        {
            Limit = 100,
            SortBy = "rating"
        });

        // Filter and match gigs based on requirements
        var matchedGigs = MatchGigsToRequirements(allGigs.Data, requirements);
        
        // Select services that fit within the total budget
        var topRecommendations = new List<RecommendedService>();
        var remainingBudget = requirements.Budget ?? decimal.MaxValue;
        
        foreach (var gig in matchedGigs)
        {
            if (gig.Price <= remainingBudget)
            {
                topRecommendations.Add(gig);
                remainingBudget -= gig.Price;
                
                // Limit to top 10 services or until budget is exhausted
                if (topRecommendations.Count >= 10 || remainingBudget <= 0)
                {
                    break;
                }
            }
        }

        // Generate project summary
        var projectSummary = new ProjectSummary
        {
            ProjectType = requirements.ProjectType ?? "General Project",
            Description = requirements.Description ?? string.Join(" ", chatSession.Messages.Where(m => m.Role == "user").Select(m => m.Content)),
            BrandName = requirements.BrandName,
            KeyRequirements = ExtractKeyRequirements(chatSession.Messages)
        };

        // Calculate budget breakdown based on top recommendations only
        var totalCost = topRecommendations.Sum(g => g.Price);
        var budgetBreakdown = new BudgetBreakdown
        {
            TotalBudget = requirements.Budget ?? 0,
            TotalCost = totalCost,
            Remaining = (requirements.Budget ?? 0) - totalCost,
            ServiceCosts = topRecommendations.Select(g => new ServiceCost
            {
                ServiceName = g.Title,
                Cost = g.Price
            }).ToList()
        };

        // Calculate timeline based on top recommendations
        var maxDeliveryDays = topRecommendations.Any() ? topRecommendations.Max(g => g.DeliveryDays) : 0;
        var timeline = new Timeline
        {
            TotalDays = maxDeliveryDays,
            DeadlineDays = requirements.DeadlineDays ?? 0,
            IsFeasible = requirements.DeadlineDays == null || maxDeliveryDays <= requirements.DeadlineDays,
            Items = topRecommendations.Select((g, index) => new TimelineItem
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
            Services = topRecommendations,
            Budget = budgetBreakdown,
            Timeline = timeline
        };
    }

    private List<RecommendedService> MatchGigsToRequirements(List<GigSummaryDto> gigs, ExtractedRequirements requirements)
    {
        var matched = new List<RecommendedService>();

        foreach (var gig in gigs)
        {
            var gigPrice = gig.Packages?.Basic?.Price ?? 0;
            var gigDeliveryDays = gig.Packages?.Basic?.DeliveryDays ?? 0;

            // Hard filters: Must meet budget and deadline constraints if specified
            if (requirements.Budget.HasValue && gigPrice > requirements.Budget.Value)
            {
                continue; // Skip gigs that exceed budget
            }

            if (requirements.DeadlineDays.HasValue && gigDeliveryDays > requirements.DeadlineDays.Value)
            {
                continue; // Skip gigs that exceed deadline
            }

            // Calculate relevance score
            var score = 0.0;
            var reasons = new List<string>();

            // Category matching (higher priority)
            if (!string.IsNullOrEmpty(requirements.ProjectType))
            {
                var projectTypeLower = requirements.ProjectType.ToLower();
                if (gig.Category?.Name?.ToLower().Contains(projectTypeLower) == true ||
                    gig.Title.ToLower().Contains(projectTypeLower))
                {
                    score += 20; // Increased weight for category match
                    reasons.Add("Matches project type");
                }
            }

            // Budget matching (within budget is already filtered, so reward efficiency)
            if (requirements.Budget.HasValue)
            {
                var budgetValue = (decimal)requirements.Budget.Value;
                var budgetEfficiency = (double)(budgetValue - gigPrice) / (double)budgetValue;
                if (budgetEfficiency > 0.5) // More than 50% of budget remaining
                {
                    score += 5;
                    reasons.Add("Excellent value");
                }
                else
                {
                    reasons.Add("Within budget");
                }
            }

            // Deadline matching (meets deadline is already filtered, so reward speed)
            if (requirements.DeadlineDays.HasValue)
            {
                var deadlineBuffer = (double)(requirements.DeadlineDays.Value - gigDeliveryDays) / requirements.DeadlineDays.Value;
                if (deadlineBuffer > 0.3) // More than 30% buffer
                {
                    score += 3;
                    reasons.Add("Fast delivery");
                }
                else
                {
                    reasons.Add("Meets deadline");
                }
            }

            // Rating boost (normalized to 0-5 range)
            score += (double)gig.Rating;

            // Review count boost (more reviews = more trusted)
            if (gig.ReviewCount > 10)
            {
                score += 2;
            }

            // Only add gigs with positive relevance
            if (score > 0 || reasons.Any())
            {
                matched.Add(new RecommendedService
                {
                    GigId = gig.Id,
                    Title = gig.Title,
                    Slug = gig.Slug,
                    Category = gig.Category?.Name ?? "Uncategorized",
                    SellerName = gig.Seller?.Username ?? "Unknown",
                    Price = gigPrice,
                    DeliveryDays = gigDeliveryDays,
                    Rating = gig.Rating,
                    ReviewCount = gig.ReviewCount,
                    ImageUrl = gig.Images?.FirstOrDefault(),
                    Reason = string.Join(", ", reasons),
                    PackageType = "basic"
                });
            }
        }

        // Sort by score (highest first), then by rating, then by price (lowest first)
        return matched
            .OrderByDescending(m => 
            {
                // Calculate score for sorting
                var s = (double)m.Rating;
                if (requirements.Budget.HasValue && m.Price <= requirements.Budget.Value * 0.5m) s += 5;
                if (requirements.DeadlineDays.HasValue && m.DeliveryDays <= requirements.DeadlineDays.Value * 0.5) s += 3;
                if (m.ReviewCount > 10) s += 2;
                return s;
            })
            .ThenByDescending(m => m.Rating)
            .ThenBy(m => m.Price)
            .ToList();
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
        // 1. Check strict indicators from AI text
        // The AI is instructed to ONLY say these phrases after the user has confirmed the summary.
        var lowerResponse = aiResponse.ToLower();
        var indicators = new[] { 
            "perfect! let me find", "recommendation", "here are the best", "searching for", "generating recommendations" 
        };
        
        if (indicators.Any(indicator => lowerResponse.Contains(indicator)) && messages.Any(m => m.Role == "user"))
        {
            return true;
        }

        // REMOVED: Data-driven heuristic (ProjectType + Budget). 
        // We now strictly require the AI to conduct the confirmation flow.
        
        return false;
    }

    private string GetSystemPrompt()
    {
        return @"You are LinkerAI, an intelligent project assistant on IsLink.
        
GOAL: match the user with the best freelance services.

PROTOCOL:
1. Ask questions ONE BY ONE to gather: Project Type, Budget (approx USD), Deadline, and Specific Requirements.
2. Once you have a rough idea, SUMMARIZE the requirements clearly to the user.
3. ASK for confirmation: ""Is this correct?""
4. ONLY after the user confirms (says yes/correct), say exactly: ""Perfect! Let me find the best services for you...""

TONE: Professional, concise, friendly. Do not output markdown lists during the gathering phase. Keep it conversational.";
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

