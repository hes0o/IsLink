using System.Net.Http.Json;
using IsLink.API.Models.DTOs;
using IsLink.API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Xunit;

namespace IsLink.API.Tests;

public sealed class RealLinkerAISmokeTests
{
    private sealed class RealAiWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                // Ensure we use the real LinkerAI implementation for this smoke test.
                services.RemoveAll<ILinkerAIService>();
                services.AddScoped<ILinkerAIService, LinkerAIService>();

                // Ensure IMongoDatabase can be created even if no real Mongo is running.
                // MongoClient won't connect until used; LinkerAIService already falls back to in-memory on failures.
                services.RemoveAll<IMongoClient>();
                services.AddSingleton<IMongoClient>(_ => new MongoClient("mongodb://localhost:27017"));
                services.RemoveAll<IMongoDatabase>();
                services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("islink"));
            });
        }
    }

    [Fact]
    public async Task RealGemini_StartConversation_Works_WhenEnabled()
    {
        var enabled = string.Equals(
            Environment.GetEnvironmentVariable("RUN_REAL_LINKERAI_SMOKE"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (!enabled)
        {
            return; // skip silently unless user opts in
        }

        var apiKey =
            Environment.GetEnvironmentVariable("Gemini__ApiKey") ??
            Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        Assert.False(string.IsNullOrWhiteSpace(apiKey), "Set Gemini__ApiKey (or GEMINI_API_KEY) before running real smoke test.");

        await using var factory = new RealAiWebAppFactory();
        var client = factory.CreateClient();

        var res = await client.PostAsJsonAsync("/api/linkerai/start", new LinkerAIStartRequest
        {
            InitialMessage = "I need an ecommerce website. Budget $500. Deadline 2 weeks."
        });

        // This is the exact thing that fails on deploy (Internal server error on opening page).
        res.EnsureSuccessStatusCode();

        var body = await res.Content.ReadFromJsonAsync<LinkerAIChatResponse>();
        Assert.NotNull(body);
        Assert.True(body!.Success);
        Assert.False(string.IsNullOrWhiteSpace(body.SessionId));
        Assert.False(string.IsNullOrWhiteSpace(body.Message));
    }
}


