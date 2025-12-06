using System.Text;
using FreelancerPlatform.Data;
using FreelancerPlatform.Mappings;
using FreelancerPlatform.Repositories;
using FreelancerPlatform.Repositories.Interfaces;
using FreelancerPlatform.Services;
using FreelancerPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FreelancerPlatform.Repositories;
using FreelancerPlatform.Repositories.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(Program));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

// DbContext (Choose the provider that suits you)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("FreelancerTempDb"));
// Repositories
builder.Services.AddScoped<IGigRepository, GigRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// TODO: Add other repositories (Orders, Reviews, Messages) as you will see later

// JWT Auth
var jwtKey = builder.Configuration["Jwt:Key"] ?? "THIS IS A VERY SECRET KEY CHANGE IT";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

// TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
