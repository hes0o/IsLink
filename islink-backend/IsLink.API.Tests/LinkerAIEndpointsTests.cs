using System.Net.Http.Json;
using IsLink.API.Models.DTOs;
using Xunit;

namespace IsLink.API.Tests;

public sealed class LinkerAIEndpointsTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public LinkerAIEndpointsTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StartConversation_ReturnsSessionAndGreeting()
    {
        var res = await _client.PostAsJsonAsync("/api/linkerai/start", new LinkerAIStartRequest
        {
            UserId = null,
            InitialMessage = "I need an ecommerce website"
        });

        res.EnsureSuccessStatusCode();

        var body = await res.Content.ReadFromJsonAsync<LinkerAIChatResponse>();
        Assert.NotNull(body);
        Assert.True(body!.Success);
        Assert.False(string.IsNullOrWhiteSpace(body.SessionId));
        Assert.Contains("LinkerAI", body.Message);
    }

    [Fact]
    public async Task Chat_WithBudgetAndDeadline_ReturnsRecommendations()
    {
        var start = await _client.PostAsJsonAsync("/api/linkerai/start", new LinkerAIStartRequest
        {
            InitialMessage = "Hi"
        });
        start.EnsureSuccessStatusCode();
        var startBody = await start.Content.ReadFromJsonAsync<LinkerAIChatResponse>();
        Assert.NotNull(startBody);
        Assert.True(startBody!.Success);

        var chat = await _client.PostAsJsonAsync("/api/linkerai/chat", new LinkerAIChatRequest
        {
            SessionId = startBody.SessionId,
            Message = "Budget is $500 and deadline is 1 month"
        });

        chat.EnsureSuccessStatusCode();
        var chatBody = await chat.Content.ReadFromJsonAsync<LinkerAIChatResponse>();
        Assert.NotNull(chatBody);
        Assert.True(chatBody!.Success);
        Assert.True(chatBody.IsComplete);
        Assert.NotNull(chatBody.Recommendations);
        Assert.NotEmpty(chatBody.Recommendations!.Services);
    }

    [Fact]
    public async Task Status_ReturnsProviderAndConfigFlags()
    {
        var res = await _client.GetAsync("/api/linkerai/status");
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        // JSON is camelCase due to ASP.NET Core defaults
        Assert.Contains("providerMode", json);
        Assert.Contains("geminiConfigured", json);
        Assert.Contains("groqConfigured", json);
    }
}


