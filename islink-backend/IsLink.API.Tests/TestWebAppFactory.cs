using IsLink.API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IsLink.API.Tests;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Ensures Program.cs skips DB init (and helps isolate LinkerAI errors)
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Replace the real LinkerAI (Gemini/Mongo/DB) with a fast deterministic fake.
            services.RemoveAll<ILinkerAIService>();
            services.AddSingleton<ILinkerAIService, FakeLinkerAIService>();
        });
    }
}


