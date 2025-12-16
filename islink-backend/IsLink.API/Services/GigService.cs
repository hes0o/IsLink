using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Services;

public class GigService : IGigService
{
    private readonly ApplicationDbContext _context;

    public GigService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GigListResponse> GetGigsAsync(GigFilterRequest filter)
    {
        var query = _context.Gigs
            .Include(g => g.Seller).ThenInclude(s => s.Skills)
            .Include(g => g.Category)
            .Include(g => g.Images)
            .Include(g => g.Tags)
            .Include(g => g.Packages)
            .Where(g => g.IsActive)
            .AsQueryable();

        // Category filter
        if (!string.IsNullOrEmpty(filter.Category))
        {
            query = query.Where(g => g.Category != null && g.Category.Slug == filter.Category);
        }

        // Search filter
        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(g => 
                g.Title.ToLower().Contains(search) ||
                g.Description.ToLower().Contains(search) ||
                g.Tags.Any(t => t.Tag.ToLower().Contains(search)));
        }

        // Price filters
        if (filter.MinPrice.HasValue)
        {
            query = query.Where(g => g.Packages.Any(p => p.PackageType == "basic" && p.Price >= filter.MinPrice));
        }
        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(g => g.Packages.Any(p => p.PackageType == "basic" && p.Price <= filter.MaxPrice));
        }

        // Delivery time filter
        if (filter.DeliveryTime.HasValue && filter.DeliveryTime > 0)
        {
            query = query.Where(g => g.Packages.Any(p => p.PackageType == "basic" && p.DeliveryDays <= filter.DeliveryTime));
        }

        // Sorting
        query = filter.SortBy switch
        {
            "price_low" => query.OrderBy(g => g.Packages.Where(p => p.PackageType == "basic").Select(p => p.Price).FirstOrDefault()),
            "price_high" => query.OrderByDescending(g => g.Packages.Where(p => p.PackageType == "basic").Select(p => p.Price).FirstOrDefault()),
            "rating" => query.OrderByDescending(g => g.Rating),
            "reviews" => query.OrderByDescending(g => g.ReviewCount),
            "newest" => query.OrderByDescending(g => g.CreatedAt),
            _ => query.OrderByDescending(g => g.Rating).ThenByDescending(g => g.ReviewCount)
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        var gigs = await query.Skip(filter.Offset).Take(filter.Limit).ToListAsync();

        return new GigListResponse
        {
            Success = true,
            Data = gigs.Select(MapToGigSummaryDto).ToList(),
            Pagination = new PaginationDto
            {
                Limit = filter.Limit,
                Offset = filter.Offset,
                Total = totalCount
            }
        };
    }

    public async Task<GigDetailResponse> GetGigBySlugAsync(string slug)
    {
        var gig = await _context.Gigs
            .Include(g => g.Seller).ThenInclude(s => s.Skills)
            .Include(g => g.Seller).ThenInclude(s => s.Languages)
            .Include(g => g.Category)
            .Include(g => g.Images.OrderBy(i => i.SortOrder))
            .Include(g => g.Tags)
            .Include(g => g.Packages).ThenInclude(p => p.Features.OrderBy(f => f.SortOrder))
            .FirstOrDefaultAsync(g => g.Slug == slug);

        if (gig == null)
        {
            return new GigDetailResponse { Success = false };
        }

        return new GigDetailResponse
        {
            Success = true,
            Data = MapToGigDto(gig)
        };
    }

    public async Task<GigDetailResponse> CreateGigAsync(Guid sellerId, CreateGigRequest request)
    {
        // Generate slug
        var slug = GenerateSlug(request.Title);

        var gig = new Gig
        {
            SellerId = sellerId,
            CategoryId = request.CategoryId,
            Title = request.Title,
            Slug = slug,
            Description = request.Description
        };

        _context.Gigs.Add(gig);

        // Add images
        for (int i = 0; i < request.Images.Count; i++)
        {
            _context.GigImages.Add(new GigImage
            {
                GigId = gig.Id,
                ImageUrl = request.Images[i],
                IsPrimary = i == 0,
                SortOrder = i
            });
        }

        // Add tags
        foreach (var tag in request.Tags)
        {
            _context.GigTags.Add(new GigTag
            {
                GigId = gig.Id,
                Tag = tag.ToLower()
            });
        }

        // Add packages
        await AddPackage(gig.Id, "basic", request.Packages.Basic);
        await AddPackage(gig.Id, "standard", request.Packages.Standard);
        await AddPackage(gig.Id, "premium", request.Packages.Premium);

        await _context.SaveChangesAsync();

        return new GigDetailResponse
        {
            Success = true,
            Data = new GigDto { Id = gig.Id, Slug = gig.Slug, Title = gig.Title }
        };
    }

    private async Task AddPackage(Guid gigId, string packageType, PackageDto dto)
    {
        var package = new GigPackage
        {
            GigId = gigId,
            PackageType = packageType,
            Name = dto.Name,
            Price = dto.Price,
            DeliveryDays = dto.DeliveryDays,
            Revisions = dto.Revisions,
            Description = dto.Description
        };

        _context.GigPackages.Add(package);

        for (int i = 0; i < dto.Features.Count; i++)
        {
            _context.PackageFeatures.Add(new PackageFeature
            {
                PackageId = package.Id,
                FeatureText = dto.Features[i],
                SortOrder = i
            });
        }

        await Task.CompletedTask;
    }

    public async Task<GigListResponse> GetGigsBySellerAsync(string username)
    {
        var gigs = await _context.Gigs
            .Include(g => g.Seller)
            .Include(g => g.Images)
            .Include(g => g.Packages)
            .Where(g => g.Seller.Username.ToLower() == username.ToLower())
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();

        return new GigListResponse
        {
            Success = true,
            Data = gigs.Select(MapToGigSummaryDto).ToList()
        };
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
        
        // Remove special characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        
        return $"{slug}-{DateTime.UtcNow.Ticks.ToString("x")}";
    }

    private static GigSummaryDto MapToGigSummaryDto(Gig gig)
    {
        return new GigSummaryDto
        {
            Id = gig.Id,
            Title = gig.Title,
            Slug = gig.Slug,
            Rating = gig.Rating,
            ReviewCount = gig.ReviewCount,
            OrdersInQueue = gig.OrdersInQueue,
            Images = gig.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
            Tags = gig.Tags.Select(t => t.Tag).ToList(),
            Packages = new GigPackagesSummaryDto
            {
                Basic = MapPackage(gig.Packages.FirstOrDefault(p => p.PackageType == "basic")),
                Standard = MapPackage(gig.Packages.FirstOrDefault(p => p.PackageType == "standard")),
                Premium = MapPackage(gig.Packages.FirstOrDefault(p => p.PackageType == "premium"))
            },
            Category = gig.Category == null ? null : new CategorySummaryDto
            {
                Id = gig.Category.Id,
                Name = gig.Category.Name,
                Slug = gig.Category.Slug
            },
            Seller = new SellerSummaryDto
            {
                Id = gig.Seller.Id,
                Username = gig.Seller.Username,
                FullName = gig.Seller.FullName,
                AvatarUrl = gig.Seller.AvatarUrl,
                Rating = gig.Seller.Rating,
                ReviewCount = gig.Seller.ReviewCount,
                IsVerified = gig.Seller.IsVerified,
                IsOnline = gig.Seller.IsOnline,
                Country = gig.Seller.Country,
                Bio = gig.Seller.Bio,
                CompletedOrders = gig.Seller.CompletedOrders,
                MemberSince = gig.Seller.CreatedAt,
                Skills = gig.Seller.Skills?.Select(s => s.SkillName).ToList() ?? new()
            },
            CreatedAt = gig.CreatedAt
        };
    }

    private static GigDto MapToGigDto(Gig gig)
    {
        var summary = MapToGigSummaryDto(gig);
        return new GigDto
        {
            Id = summary.Id,
            Title = summary.Title,
            Slug = summary.Slug,
            Description = gig.Description,
            Rating = summary.Rating,
            ReviewCount = summary.ReviewCount,
            OrdersInQueue = summary.OrdersInQueue,
            Images = summary.Images,
            Tags = summary.Tags,
            Packages = new GigPackagesSummaryDto
            {
                Basic = MapPackageWithFeatures(gig.Packages.FirstOrDefault(p => p.PackageType == "basic")),
                Standard = MapPackageWithFeatures(gig.Packages.FirstOrDefault(p => p.PackageType == "standard")),
                Premium = MapPackageWithFeatures(gig.Packages.FirstOrDefault(p => p.PackageType == "premium"))
            },
            Category = summary.Category,
            Seller = new SellerSummaryDto
            {
                Id = gig.Seller.Id,
                Username = gig.Seller.Username,
                FullName = gig.Seller.FullName,
                AvatarUrl = gig.Seller.AvatarUrl,
                Rating = gig.Seller.Rating,
                ReviewCount = gig.Seller.ReviewCount,
                IsVerified = gig.Seller.IsVerified,
                IsOnline = gig.Seller.IsOnline,
                Country = gig.Seller.Country,
                Bio = gig.Seller.Bio,
                CompletedOrders = gig.Seller.CompletedOrders,
                MemberSince = gig.Seller.CreatedAt,
                Skills = gig.Seller.Skills?.Select(s => s.SkillName).ToList() ?? new(),
                Languages = gig.Seller.Languages?.Select(l => new LanguageDto 
                { 
                    Name = l.LanguageName, 
                    Level = l.Proficiency ?? "" 
                }).ToList() ?? new()
            },
            CreatedAt = summary.CreatedAt,
            IsActive = gig.IsActive
        };
    }

    private static PackageSummaryDto? MapPackage(GigPackage? package)
    {
        if (package == null) return null;
        return new PackageSummaryDto
        {
            Name = package.Name,
            Price = package.Price,
            DeliveryDays = package.DeliveryDays,
            Revisions = package.Revisions
        };
    }

    private static PackageSummaryDto? MapPackageWithFeatures(GigPackage? package)
    {
        if (package == null) return null;
        return new PackageSummaryDto
        {
            Name = package.Name,
            Price = package.Price,
            DeliveryDays = package.DeliveryDays,
            Revisions = package.Revisions,
            Description = package.Description,
            Features = package.Features.OrderBy(f => f.SortOrder).Select(f => f.FeatureText).ToList()
        };
    }
}

