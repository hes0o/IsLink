using IsLink.API.Data;
using IsLink.API.Services;
using IsLink.API.Middleware;
using IsLink.API.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// Configuration
// ============================================

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));

// ============================================
// Database Contexts
// ============================================

// PostgreSQL with Entity Framework Core
if (builder.Environment.IsEnvironment("Test"))
{
    // Integration/smoke tests should not require an external Postgres instance.
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("IsLink_TestDb"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
}

// MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    // Use a safe default so the API can boot even if MongoDB conn string is missing in some environments.
    var mongoConn = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
    return new MongoClient(mongoConn);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("islink");
});

// ============================================
// Services
// ============================================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGigService, GigService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// HttpClient for Groq API (LinkerAI)
builder.Services.AddHttpClient();
builder.Services.AddScoped<ILinkerAIService, LinkerAIService>();

// ============================================
// Authentication (JWT)
// ============================================

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings!.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ============================================
// CORS
// ============================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
              {
                  // Allow localhost for development
                  if (origin.Contains("localhost")) return true;
                  // Allow Vercel deployments
                  if (origin.EndsWith(".vercel.app")) return true;
                  // Allow Render deployments
                  if (origin.EndsWith(".onrender.com")) return true;
                  return false;
              })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ============================================
// Controllers & Swagger
// ============================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IsLink API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// ============================================
// Middleware Pipeline
// ============================================

// Swagger UI - Always enabled for testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IsLink API v1");
    c.RoutePrefix = "swagger";
});

// Enable static file serving
app.UseStaticFiles();

// CORS
app.UseCors("AllowFrontend");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Custom Exception Handling Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Map Controllers
app.MapControllers();

// Root endpoint
app.MapGet("/", () => Results.Ok(new
{
    Message = "Welcome to IsLink API",
    Version = "1.0.0",
    Documentation = "/swagger"
}));

// Health check
app.MapGet("/api/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow
}));

// ============================================
// Initialize Database
// ============================================

// For local/integration tests we want fast boot and no external DB requirement.
// - Set ASPNETCORE_ENVIRONMENT=Test (recommended for tests), or
// - Set SKIP_DB_INIT=true (useful for quick local smoke runs)
var skipDbInit =
    app.Environment.IsEnvironment("Test") ||
    string.Equals(Environment.GetEnvironmentVariable("SKIP_DB_INIT"), "true", StringComparison.OrdinalIgnoreCase);

if (!skipDbInit)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            // Run migrations
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("✅ Database migrated successfully");

            // Seed database with demo data
            try
            {
                await DbSeeder.SeedAsync(dbContext);
                Console.WriteLine("✅ Database seeded successfully");
            }
            catch (Exception seedEx)
            {
                // Don't fail startup if seeding fails
                Console.WriteLine($"⚠️ Database seeding warning (non-fatal): {seedEx.Message}");
            }
        }
        catch (Exception ex)
        {
            // Don't fail startup if migration/seeding fails
            Console.WriteLine($"⚠️ Database initialization warning (non-fatal): {ex.Message}");
        }
    }
}
else
{
    Console.WriteLine("ℹ️ Skipping DB initialization (Test env / SKIP_DB_INIT=true)");
}

Console.WriteLine(@"
╔════════════════════════════════════════════════════╗
║                                                    ║
║   🚀 IsLink API Server (.NET 8)                    ║
║                                                    ║
║   Swagger:  http://localhost:5000/swagger          ║
║   API:      http://localhost:5000/api              ║
║                                                    ║
╚════════════════════════════════════════════════════╝
");

app.Run();

// Required for integration tests via WebApplicationFactory<T>
public partial class Program { }

