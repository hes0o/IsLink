using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if already seeded (check for users since categories might have been partially seeded)
        if (await context.Users.AnyAsync(u => u.Username == "sarah_designs"))
        {
            Console.WriteLine("📦 Database already seeded");
            return;
        }

        Console.WriteLine("🌱 Seeding database...");

        // ============================================
        // CATEGORIES
        // ============================================
        var categories = new List<Category>
        {
            new() { Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"), Name = "Graphics & Design", Slug = "graphics-design", Icon = "🎨" },
            new() { Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"), Name = "Programming & Tech", Slug = "programming-tech", Icon = "💻" },
            new() { Id = Guid.Parse("c3333333-3333-3333-3333-333333333333"), Name = "Digital Marketing", Slug = "digital-marketing", Icon = "📱" },
            new() { Id = Guid.Parse("c4444444-4444-4444-4444-444444444444"), Name = "Writing & Translation", Slug = "writing-translation", Icon = "✍️" },
            new() { Id = Guid.Parse("c5555555-5555-5555-5555-555555555555"), Name = "Video & Animation", Slug = "video-animation", Icon = "🎬" },
            new() { Id = Guid.Parse("c6666666-6666-6666-6666-666666666666"), Name = "Music & Audio", Slug = "music-audio", Icon = "🎵" },
            new() { Id = Guid.Parse("c7777777-7777-7777-7777-777777777777"), Name = "Business", Slug = "business", Icon = "💼" },
            new() { Id = Guid.Parse("c8888888-8888-8888-8888-888888888888"), Name = "AI Services", Slug = "ai-services", Icon = "🤖" },
        };

        // Subcategories
        var subcategories = new List<Category>
        {
            // Graphics & Design subcategories
            new() { Id = Guid.NewGuid(), Name = "Logo Design", Slug = "logo-design", ParentId = Guid.Parse("c1111111-1111-1111-1111-111111111111") },
            new() { Id = Guid.NewGuid(), Name = "Brand Style Guides", Slug = "brand-style-guides", ParentId = Guid.Parse("c1111111-1111-1111-1111-111111111111") },
            new() { Id = Guid.NewGuid(), Name = "UI/UX Design", Slug = "ui-ux-design", ParentId = Guid.Parse("c1111111-1111-1111-1111-111111111111") },
            
            // Programming & Tech subcategories
            new() { Id = Guid.NewGuid(), Name = "Web Development", Slug = "web-development", ParentId = Guid.Parse("c2222222-2222-2222-2222-222222222222") },
            new() { Id = Guid.NewGuid(), Name = "Mobile Apps", Slug = "mobile-apps", ParentId = Guid.Parse("c2222222-2222-2222-2222-222222222222") },
            new() { Id = Guid.NewGuid(), Name = "E-Commerce Development", Slug = "ecommerce-development", ParentId = Guid.Parse("c2222222-2222-2222-2222-222222222222") },
            
            // Digital Marketing subcategories
            new() { Id = Guid.NewGuid(), Name = "Social Media Marketing", Slug = "social-media-marketing", ParentId = Guid.Parse("c3333333-3333-3333-3333-333333333333") },
            new() { Id = Guid.NewGuid(), Name = "SEO", Slug = "seo", ParentId = Guid.Parse("c3333333-3333-3333-3333-333333333333") },
        };

        await context.Categories.AddRangeAsync(categories);
        await context.Categories.AddRangeAsync(subcategories);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Categories seeded");

        // ============================================
        // USERS (Sellers & Buyers)
        // ============================================
        var users = new List<User>
        {
            // Sellers
            new()
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                Username = "sarah_designs",
                Email = "sarah@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "Sarah Al-Ahmad",
                AvatarUrl = "https://randomuser.me/api/portraits/women/44.jpg",
                Role = "seller",
                Country = "Syria",
                Bio = "Professional graphic designer with 5+ years of experience. Specializing in logo design, branding, and UI/UX. I create unique designs that help businesses stand out.",
                Rating = 4.9m,
                ReviewCount = 127,
                CompletedOrders = 156,
                IsOnline = true,
                IsVerified = true,
                Balance = 2450.00m,
                Skills = new List<UserSkill>
                {
                    new() { SkillName = "Logo Design" },
                    new() { SkillName = "Branding" },
                    new() { SkillName = "UI/UX Design" },
                    new() { SkillName = "Adobe Photoshop" },
                    new() { SkillName = "Adobe Illustrator" }
                },
                Languages = new List<UserLanguage>
                {
                    new() { LanguageName = "Arabic", Proficiency = "Native" },
                    new() { LanguageName = "English", Proficiency = "Fluent" }
                }
            },
            new()
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                Username = "ahmed_dev",
                Email = "ahmed@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "Ahmed Hassan",
                AvatarUrl = "https://randomuser.me/api/portraits/men/75.jpg",
                Role = "seller",
                Country = "Syria",
                Bio = "Full-stack developer with expertise in React, Node.js, and .NET. I build scalable web applications and love solving complex problems.",
                Rating = 5.0m,
                ReviewCount = 89,
                CompletedOrders = 112,
                IsOnline = true,
                IsVerified = true,
                Balance = 3200.00m,
                Skills = new List<UserSkill>
                {
                    new() { SkillName = "React" },
                    new() { SkillName = "Node.js" },
                    new() { SkillName = ".NET" },
                    new() { SkillName = "TypeScript" },
                    new() { SkillName = "PostgreSQL" }
                },
                Languages = new List<UserLanguage>
                {
                    new() { LanguageName = "Arabic", Proficiency = "Native" },
                    new() { LanguageName = "English", Proficiency = "Fluent" },
                    new() { LanguageName = "Turkish", Proficiency = "Conversational" }
                }
            },
            new()
            {
                Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                Username = "maria_writer",
                Email = "maria@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "Maria Gonzalez",
                AvatarUrl = "https://randomuser.me/api/portraits/women/68.jpg",
                Role = "seller",
                Country = "Spain",
                Bio = "Professional content writer and translator. I specialize in SEO content, blog posts, and English-Spanish translation.",
                Rating = 4.8m,
                ReviewCount = 64,
                CompletedOrders = 78,
                IsOnline = false,
                IsVerified = true,
                Balance = 1800.00m,
                Skills = new List<UserSkill>
                {
                    new() { SkillName = "Content Writing" },
                    new() { SkillName = "SEO Writing" },
                    new() { SkillName = "Translation" },
                    new() { SkillName = "Copywriting" }
                },
                Languages = new List<UserLanguage>
                {
                    new() { LanguageName = "Spanish", Proficiency = "Native" },
                    new() { LanguageName = "English", Proficiency = "Fluent" }
                }
            },
            new()
            {
                Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                Username = "alex_motion",
                Email = "alex@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "Alex Turner",
                AvatarUrl = "https://randomuser.me/api/portraits/men/32.jpg",
                Role = "seller",
                Country = "United Kingdom",
                Bio = "Motion graphics designer and video editor. Creating stunning animations and videos for brands worldwide.",
                Rating = 4.7m,
                ReviewCount = 45,
                CompletedOrders = 52,
                IsOnline = true,
                IsVerified = true,
                Balance = 1200.00m,
                Skills = new List<UserSkill>
                {
                    new() { SkillName = "After Effects" },
                    new() { SkillName = "Premiere Pro" },
                    new() { SkillName = "Motion Graphics" },
                    new() { SkillName = "Video Editing" }
                },
                Languages = new List<UserLanguage>
                {
                    new() { LanguageName = "English", Proficiency = "Native" }
                }
            },
            // Buyers
            new()
            {
                Id = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                Username = "john_buyer",
                Email = "john@client.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "John Smith",
                AvatarUrl = "https://randomuser.me/api/portraits/men/52.jpg",
                Role = "buyer",
                Country = "United States",
                Bio = "Startup founder looking for talented freelancers.",
                Rating = 4.8m,
                ReviewCount = 23,
                CompletedOrders = 28,
                IsOnline = true,
                IsVerified = true,
                Balance = 500.00m
            },
            new()
            {
                Id = Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                Username = "emma_client",
                Email = "emma@client.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "Emma Wilson",
                AvatarUrl = "https://randomuser.me/api/portraits/women/33.jpg",
                Role = "buyer",
                Country = "Canada",
                Bio = "Marketing manager at a tech company.",
                Rating = 5.0m,
                ReviewCount = 15,
                CompletedOrders = 18,
                IsOnline = false,
                IsVerified = true,
                Balance = 350.00m
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Users seeded");

        // ============================================
        // GIGS
        // ============================================
        var gigs = new List<Gig>
        {
            // Sarah's Gigs (Graphics & Design)
            new()
            {
                Id = Guid.Parse("g1111111-1111-1111-1111-111111111111"),
                SellerId = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                CategoryId = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                Title = "I will design a professional logo for your business",
                Slug = "professional-logo-design-business",
                Description = "Looking for a unique, professional logo that will make your brand stand out? You're in the right place!\n\nWith over 5 years of experience in graphic design, I've helped hundreds of businesses create memorable brand identities. I specialize in minimalist, modern, and versatile logo designs that work across all platforms.\n\nWhat you'll get:\n- 100% original design\n- Unlimited revisions until you're satisfied\n- All source files (AI, EPS, PDF, PNG, JPG)\n- Full commercial rights\n- Fast delivery",
                Rating = 4.9m,
                ReviewCount = 127,
                OrdersInQueue = 5,
                IsActive = true,
                Images = new List<GigImage>
                {
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/q_auto,f_auto/gigs/103229843/original/a']logo1.jpg", IsPrimary = true, SortOrder = 0 },
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/q_auto,f_auto/gigs2/103229843/original/logo2.jpg", IsPrimary = false, SortOrder = 1 }
                },
                Tags = new List<GigTag>
                {
                    new() { Tag = "logo design" },
                    new() { Tag = "branding" },
                    new() { Tag = "minimalist" },
                    new() { Tag = "business logo" }
                },
                Packages = new List<GigPackage>
                {
                    new()
                    {
                        PackageType = "basic",
                        Name = "Basic",
                        Price = 25.00m,
                        DeliveryDays = 3,
                        Revisions = "2",
                        Description = "1 concept, PNG & JPG files",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "1 Logo Concept", SortOrder = 0 },
                            new() { FeatureText = "PNG & JPG Files", SortOrder = 1 },
                            new() { FeatureText = "2 Revisions", SortOrder = 2 }
                        }
                    },
                    new()
                    {
                        PackageType = "standard",
                        Name = "Standard",
                        Price = 50.00m,
                        DeliveryDays = 2,
                        Revisions = "5",
                        Description = "2 concepts, all source files",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "2 Logo Concepts", SortOrder = 0 },
                            new() { FeatureText = "All Source Files (AI, EPS, PDF)", SortOrder = 1 },
                            new() { FeatureText = "PNG & JPG Files", SortOrder = 2 },
                            new() { FeatureText = "5 Revisions", SortOrder = 3 }
                        }
                    },
                    new()
                    {
                        PackageType = "premium",
                        Name = "Premium",
                        Price = 100.00m,
                        DeliveryDays = 1,
                        Revisions = "Unlimited",
                        Description = "3 concepts, full branding kit",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "3 Logo Concepts", SortOrder = 0 },
                            new() { FeatureText = "Full Source Files", SortOrder = 1 },
                            new() { FeatureText = "Social Media Kit", SortOrder = 2 },
                            new() { FeatureText = "Business Card Design", SortOrder = 3 },
                            new() { FeatureText = "Unlimited Revisions", SortOrder = 4 }
                        }
                    }
                }
            },
            new()
            {
                Id = Guid.Parse("g1111111-1111-1111-1111-111111111112"),
                SellerId = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                CategoryId = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                Title = "I will create a modern UI/UX design for your app",
                Slug = "modern-ui-ux-design-app",
                Description = "Need a stunning user interface for your mobile or web application? I'll create a modern, user-friendly design that your users will love.\n\nI follow the latest design trends and best practices to ensure your app looks professional and provides an excellent user experience.",
                Rating = 4.8m,
                ReviewCount = 45,
                OrdersInQueue = 3,
                IsActive = true,
                Tags = new List<GigTag>
                {
                    new() { Tag = "ui design" },
                    new() { Tag = "ux design" },
                    new() { Tag = "mobile app" },
                    new() { Tag = "figma" }
                },
                Packages = new List<GigPackage>
                {
                    new() { PackageType = "basic", Name = "Basic", Price = 75.00m, DeliveryDays = 5, Revisions = "2" },
                    new() { PackageType = "standard", Name = "Standard", Price = 150.00m, DeliveryDays = 3, Revisions = "5" },
                    new() { PackageType = "premium", Name = "Premium", Price = 300.00m, DeliveryDays = 2, Revisions = "Unlimited" }
                }
            },
            // Ahmed's Gigs (Programming)
            new()
            {
                Id = Guid.Parse("g2222222-2222-2222-2222-222222222221"),
                SellerId = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                CategoryId = Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                Title = "I will build a professional React website for you",
                Slug = "professional-react-website-development",
                Description = "Looking for a fast, modern, and responsive website? I specialize in React.js development and can build anything from landing pages to complex web applications.\n\nWith 5+ years of experience, I deliver clean, maintainable code that scales with your business.",
                Rating = 5.0m,
                ReviewCount = 89,
                OrdersInQueue = 4,
                IsActive = true,
                Tags = new List<GigTag>
                {
                    new() { Tag = "react" },
                    new() { Tag = "web development" },
                    new() { Tag = "javascript" },
                    new() { Tag = "frontend" }
                },
                Packages = new List<GigPackage>
                {
                    new() { PackageType = "basic", Name = "Basic", Price = 100.00m, DeliveryDays = 7, Revisions = "2" },
                    new() { PackageType = "standard", Name = "Standard", Price = 250.00m, DeliveryDays = 5, Revisions = "5" },
                    new() { PackageType = "premium", Name = "Premium", Price = 500.00m, DeliveryDays = 3, Revisions = "Unlimited" }
                }
            },
            new()
            {
                Id = Guid.Parse("g2222222-2222-2222-2222-222222222222"),
                SellerId = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                CategoryId = Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                Title = "I will develop a full-stack web application with .NET and React",
                Slug = "fullstack-dotnet-react-application",
                Description = "Need a complete web application with both frontend and backend? I'll build it using modern technologies: React for the frontend and .NET Core for the backend, with PostgreSQL or SQL Server for the database.",
                Rating = 5.0m,
                ReviewCount = 34,
                OrdersInQueue = 2,
                IsActive = true,
                Tags = new List<GigTag>
                {
                    new() { Tag = ".net" },
                    new() { Tag = "full-stack" },
                    new() { Tag = "api development" },
                    new() { Tag = "database" }
                },
                Packages = new List<GigPackage>
                {
                    new() { PackageType = "basic", Name = "Basic", Price = 300.00m, DeliveryDays = 14, Revisions = "2" },
                    new() { PackageType = "standard", Name = "Standard", Price = 600.00m, DeliveryDays = 10, Revisions = "5" },
                    new() { PackageType = "premium", Name = "Premium", Price = 1200.00m, DeliveryDays = 7, Revisions = "Unlimited" }
                }
            },
            // Maria's Gigs (Writing)
            new()
            {
                Id = Guid.Parse("g3333333-3333-3333-3333-333333333331"),
                SellerId = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                CategoryId = Guid.Parse("c4444444-4444-4444-4444-444444444444"),
                Title = "I will write SEO-optimized blog posts and articles",
                Slug = "seo-optimized-blog-posts-articles",
                Description = "Need content that ranks? I'll write engaging, SEO-optimized articles that drive traffic to your website. With extensive experience in content marketing, I know what it takes to create content that both readers and search engines love.",
                Rating = 4.8m,
                ReviewCount = 64,
                OrdersInQueue = 6,
                IsActive = true,
                Tags = new List<GigTag>
                {
                    new() { Tag = "seo writing" },
                    new() { Tag = "blog posts" },
                    new() { Tag = "content writing" },
                    new() { Tag = "articles" }
                },
                Packages = new List<GigPackage>
                {
                    new() { PackageType = "basic", Name = "Basic", Price = 30.00m, DeliveryDays = 2, Revisions = "2" },
                    new() { PackageType = "standard", Name = "Standard", Price = 60.00m, DeliveryDays = 2, Revisions = "3" },
                    new() { PackageType = "premium", Name = "Premium", Price = 120.00m, DeliveryDays = 1, Revisions = "Unlimited" }
                }
            },
            // Alex's Gigs (Video)
            new()
            {
                Id = Guid.Parse("g4444444-4444-4444-4444-444444444441"),
                SellerId = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                CategoryId = Guid.Parse("c5555555-5555-5555-5555-555555555555"),
                Title = "I will create stunning motion graphics and animations",
                Slug = "stunning-motion-graphics-animations",
                Description = "Bring your brand to life with professional motion graphics! I create eye-catching animations for social media, advertisements, explainer videos, and more.",
                Rating = 4.7m,
                ReviewCount = 45,
                OrdersInQueue = 3,
                IsActive = true,
                Tags = new List<GigTag>
                {
                    new() { Tag = "motion graphics" },
                    new() { Tag = "animation" },
                    new() { Tag = "after effects" },
                    new() { Tag = "video" }
                },
                Packages = new List<GigPackage>
                {
                    new() { PackageType = "basic", Name = "Basic", Price = 50.00m, DeliveryDays = 3, Revisions = "2" },
                    new() { PackageType = "standard", Name = "Standard", Price = 100.00m, DeliveryDays = 2, Revisions = "3" },
                    new() { PackageType = "premium", Name = "Premium", Price = 200.00m, DeliveryDays = 1, Revisions = "Unlimited" }
                }
            }
        };

        await context.Gigs.AddRangeAsync(gigs);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Gigs seeded");

        // ============================================
        // ORDERS & REVIEWS
        // ============================================
        var orders = new List<Order>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ISL-2024-001",
                GigId = Guid.Parse("g1111111-1111-1111-1111-111111111111"),
                BuyerId = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                SellerId = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                PackageType = "standard",
                Price = 50.00m,
                ServiceFee = 5.00m,
                TotalPrice = 55.00m,
                Status = "completed",
                CompletedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ISL-2024-002",
                GigId = Guid.Parse("g2222222-2222-2222-2222-222222222221"),
                BuyerId = Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                SellerId = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                PackageType = "premium",
                Price = 500.00m,
                ServiceFee = 50.00m,
                TotalPrice = 550.00m,
                Status = "completed",
                CompletedAt = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ISL-2024-003",
                GigId = Guid.Parse("g1111111-1111-1111-1111-111111111111"),
                BuyerId = Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                SellerId = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                PackageType = "basic",
                Price = 25.00m,
                ServiceFee = 2.50m,
                TotalPrice = 27.50m,
                Status = "in_progress",
                DeliveryDeadline = DateTime.UtcNow.AddDays(3)
            }
        };

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();

        // Add reviews for completed orders
        var reviews = new List<Review>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = orders[0].Id,
                GigId = Guid.Parse("g1111111-1111-1111-1111-111111111111"),
                BuyerId = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                SellerId = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                Rating = 5,
                Comment = "Absolutely amazing work! Sarah delivered exactly what I envisioned. The logo is modern, clean, and perfect for my brand. Highly recommended!",
                SellerResponse = "Thank you so much, John! It was a pleasure working with you. Best of luck with your business!"
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = orders[1].Id,
                GigId = Guid.Parse("g2222222-2222-2222-2222-222222222221"),
                BuyerId = Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                SellerId = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                Rating = 5,
                Comment = "Ahmed is an exceptional developer! He built our entire web application from scratch and it works flawlessly. Great communication throughout the project.",
                SellerResponse = "Thank you Emma! I really enjoyed working on this project. Don't hesitate to reach out if you need any updates!"
            }
        };

        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Orders & Reviews seeded");

        Console.WriteLine("🎉 Database seeding completed!");
    }
}

