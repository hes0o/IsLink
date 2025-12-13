using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if gigs already seeded (more specific check)
        if (await context.Gigs.AnyAsync())
        {
            Console.WriteLine("ℹ️ Database already has gigs - skipping seed");
            return; // Already seeded
        }
        
        // Clear existing categories to re-seed properly
        var existingCategories = await context.Categories.ToListAsync();
        if (existingCategories.Any())
        {
            context.Categories.RemoveRange(existingCategories);
            await context.SaveChangesAsync();
        }

        Console.WriteLine("🌱 Seeding database...");

        // ============================================
        // CATEGORIES
        // ============================================
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Graphics & Design", Slug = "graphics-design", Icon = "🎨" },
            new() { Id = Guid.NewGuid(), Name = "Programming & Tech", Slug = "programming-tech", Icon = "💻" },
            new() { Id = Guid.NewGuid(), Name = "Digital Marketing", Slug = "digital-marketing", Icon = "📱" },
            new() { Id = Guid.NewGuid(), Name = "Writing & Translation", Slug = "writing-translation", Icon = "✍️" },
            new() { Id = Guid.NewGuid(), Name = "Video & Animation", Slug = "video-animation", Icon = "🎬" },
            new() { Id = Guid.NewGuid(), Name = "Music & Audio", Slug = "music-audio", Icon = "🎵" },
            new() { Id = Guid.NewGuid(), Name = "Business", Slug = "business", Icon = "💼" },
            new() { Id = Guid.NewGuid(), Name = "AI Services", Slug = "ai-services", Icon = "🤖" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // ============================================
        // USERS (Sellers)
        // ============================================
        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Username = "sarah_designer",
                Email = "sarah@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Sarah Al-Ahmad",
                AvatarUrl = "https://randomuser.me/api/portraits/women/44.jpg",
                Role = "seller",
                Country = "Syria",
                Bio = "Professional graphic designer with 5+ years of experience. Specializing in logo design, brand identity, and UI/UX design.",
                Rating = 4.9m,
                ReviewCount = 127,
                CompletedOrders = 156,
                IsOnline = true,
                IsVerified = true,
                Balance = 2450.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "ahmed_dev",
                Email = "ahmed@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Ahmed Hassan",
                AvatarUrl = "https://randomuser.me/api/portraits/men/75.jpg",
                Role = "seller",
                Country = "Syria",
                Bio = "Full-stack developer with expertise in React, Node.js, and .NET. Building scalable web applications for businesses worldwide.",
                Rating = 5.0m,
                ReviewCount = 89,
                CompletedOrders = 112,
                IsOnline = true,
                IsVerified = true,
                Balance = 3200.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "maya_writer",
                Email = "maya@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Maya Khoury",
                AvatarUrl = "https://randomuser.me/api/portraits/women/68.jpg",
                Role = "seller",
                Country = "Lebanon",
                Bio = "Professional content writer and translator. Native Arabic speaker fluent in English and French. SEO-optimized content that converts.",
                Rating = 4.8m,
                ReviewCount = 64,
                CompletedOrders = 78,
                IsOnline = false,
                IsVerified = true,
                Balance = 1800.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "omar_video",
                Email = "omar@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Omar Mansour",
                AvatarUrl = "https://randomuser.me/api/portraits/men/32.jpg",
                Role = "seller",
                Country = "Jordan",
                Bio = "Video editor and motion graphics artist. Creating stunning videos for YouTube, social media, and commercial projects.",
                Rating = 4.7m,
                ReviewCount = 45,
                CompletedOrders = 52,
                IsOnline = true,
                IsVerified = false,
                Balance = 950.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "lina_marketing",
                Email = "lina@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Lina Farah",
                AvatarUrl = "https://randomuser.me/api/portraits/women/22.jpg",
                Role = "seller",
                Country = "UAE",
                Bio = "Digital marketing expert specializing in social media management, SEO, and paid advertising. Helping businesses grow online.",
                Rating = 4.9m,
                ReviewCount = 93,
                CompletedOrders = 134,
                IsOnline = true,
                IsVerified = true,
                Balance = 4100.00m
            }
        };

        // Add skills to users
        users[0].Skills = new List<UserSkill>
        {
            new() { SkillName = "Logo Design" },
            new() { SkillName = "Brand Identity" },
            new() { SkillName = "UI/UX Design" },
            new() { SkillName = "Adobe Photoshop" },
            new() { SkillName = "Adobe Illustrator" }
        };

        users[1].Skills = new List<UserSkill>
        {
            new() { SkillName = "React" },
            new() { SkillName = "Node.js" },
            new() { SkillName = ".NET" },
            new() { SkillName = "PostgreSQL" },
            new() { SkillName = "MongoDB" }
        };

        users[2].Skills = new List<UserSkill>
        {
            new() { SkillName = "Content Writing" },
            new() { SkillName = "Translation" },
            new() { SkillName = "SEO Writing" },
            new() { SkillName = "Copywriting" }
        };

        users[3].Skills = new List<UserSkill>
        {
            new() { SkillName = "Video Editing" },
            new() { SkillName = "Motion Graphics" },
            new() { SkillName = "After Effects" },
            new() { SkillName = "Premiere Pro" }
        };

        users[4].Skills = new List<UserSkill>
        {
            new() { SkillName = "Social Media Marketing" },
            new() { SkillName = "SEO" },
            new() { SkillName = "Google Ads" },
            new() { SkillName = "Facebook Ads" }
        };

        // Add languages
        foreach (var user in users)
        {
            user.Languages = new List<UserLanguage>
            {
                new() { LanguageName = "Arabic", Proficiency = "Native" },
                new() { LanguageName = "English", Proficiency = "Fluent" }
            };
        }

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // ============================================
        // GIGS
        // ============================================
        var graphicsCategory = categories.First(c => c.Slug == "graphics-design");
        var programmingCategory = categories.First(c => c.Slug == "programming-tech");
        var writingCategory = categories.First(c => c.Slug == "writing-translation");
        var videoCategory = categories.First(c => c.Slug == "video-animation");
        var marketingCategory = categories.First(c => c.Slug == "digital-marketing");

        var gigs = new List<Gig>
        {
            // Sarah's Gigs (Designer)
            new()
            {
                Id = Guid.NewGuid(),
                SellerId = users[0].Id,
                CategoryId = graphicsCategory.Id,
                Title = "I will design a modern minimalist logo for your business",
                Slug = "modern-minimalist-logo-design",
                Description = "Looking for a professional logo that represents your brand? I create modern, minimalist logos that are memorable and versatile. Each logo is crafted with attention to detail and designed to work across all platforms - from business cards to billboards.",
                Rating = 4.9m,
                ReviewCount = 87,
                OrdersInQueue = 5,
                IsActive = true,
                Images = new List<GigImage>
                {
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/115528926/original/77cc7c95f5ebe4e46e84a5a59ac5d8e881f99b70/design-3-modern-minimalist-logo-design.png", IsPrimary = true, SortOrder = 0 },
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs2/115528926/original/a64a4d04fec1f9a16e72b9c7ea3a8f8f15c0506e/design-3-modern-minimalist-logo-design.png", IsPrimary = false, SortOrder = 1 }
                },
                Tags = new List<GigTag>
                {
                    new() { Tag = "logo design" },
                    new() { Tag = "minimalist" },
                    new() { Tag = "branding" },
                    new() { Tag = "modern" }
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
                        Description = "1 simple logo concept",
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
                        DeliveryDays = 5,
                        Revisions = "5",
                        Description = "2 logo concepts with more options",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "2 Logo Concepts", SortOrder = 0 },
                            new() { FeatureText = "PNG, JPG & Vector Files", SortOrder = 1 },
                            new() { FeatureText = "5 Revisions", SortOrder = 2 },
                            new() { FeatureText = "Source File", SortOrder = 3 }
                        }
                    },
                    new()
                    {
                        PackageType = "premium",
                        Name = "Premium",
                        Price = 100.00m,
                        DeliveryDays = 7,
                        Revisions = "Unlimited",
                        Description = "Complete brand identity package",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "3 Logo Concepts", SortOrder = 0 },
                            new() { FeatureText = "All File Formats", SortOrder = 1 },
                            new() { FeatureText = "Unlimited Revisions", SortOrder = 2 },
                            new() { FeatureText = "Source Files", SortOrder = 3 },
                            new() { FeatureText = "Brand Guidelines", SortOrder = 4 },
                            new() { FeatureText = "Social Media Kit", SortOrder = 5 }
                        }
                    }
                }
            },

            // Ahmed's Gigs (Developer)
            new()
            {
                Id = Guid.NewGuid(),
                SellerId = users[1].Id,
                CategoryId = programmingCategory.Id,
                Title = "I will build a responsive website using React and Node.js",
                Slug = "responsive-react-nodejs-website",
                Description = "Need a modern, fast, and responsive website? I specialize in building full-stack web applications using React for the frontend and Node.js for the backend. Clean code, best practices, and SEO-friendly structure included.",
                Rating = 5.0m,
                ReviewCount = 64,
                OrdersInQueue = 3,
                IsActive = true,
                Images = new List<GigImage>
                {
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/197495498/original/43c71bce96e57eeb9efaa5c3e433c3a309b0a59a/develop-modern-responsive-website-in-react-js.jpg", IsPrimary = true, SortOrder = 0 }
                },
                Tags = new List<GigTag>
                {
                    new() { Tag = "react" },
                    new() { Tag = "nodejs" },
                    new() { Tag = "web development" },
                    new() { Tag = "responsive" }
                },
                Packages = new List<GigPackage>
                {
                    new()
                    {
                        PackageType = "basic",
                        Name = "Basic",
                        Price = 100.00m,
                        DeliveryDays = 7,
                        Revisions = "2",
                        Description = "Single page website",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "1 Page", SortOrder = 0 },
                            new() { FeatureText = "Responsive Design", SortOrder = 1 },
                            new() { FeatureText = "Contact Form", SortOrder = 2 }
                        }
                    },
                    new()
                    {
                        PackageType = "standard",
                        Name = "Standard",
                        Price = 250.00m,
                        DeliveryDays = 14,
                        Revisions = "5",
                        Description = "Multi-page website with backend",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "5 Pages", SortOrder = 0 },
                            new() { FeatureText = "Responsive Design", SortOrder = 1 },
                            new() { FeatureText = "Database Integration", SortOrder = 2 },
                            new() { FeatureText = "Admin Panel", SortOrder = 3 }
                        }
                    },
                    new()
                    {
                        PackageType = "premium",
                        Name = "Premium",
                        Price = 500.00m,
                        DeliveryDays = 21,
                        Revisions = "Unlimited",
                        Description = "Full web application",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "10+ Pages", SortOrder = 0 },
                            new() { FeatureText = "User Authentication", SortOrder = 1 },
                            new() { FeatureText = "Payment Integration", SortOrder = 2 },
                            new() { FeatureText = "API Development", SortOrder = 3 },
                            new() { FeatureText = "Deployment Included", SortOrder = 4 }
                        }
                    }
                }
            },

            // Maya's Gigs (Writer)
            new()
            {
                Id = Guid.NewGuid(),
                SellerId = users[2].Id,
                CategoryId = writingCategory.Id,
                Title = "I will write SEO optimized blog posts and articles",
                Slug = "seo-optimized-blog-posts-articles",
                Description = "Get high-quality, SEO-optimized content that ranks! I write engaging blog posts and articles that drive traffic to your website. Native Arabic speaker with excellent English skills.",
                Rating = 4.8m,
                ReviewCount = 52,
                OrdersInQueue = 8,
                IsActive = true,
                Images = new List<GigImage>
                {
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/103aborede/original/eb5f96a5f8a0b890c05c9e43d4f8d77bb8b8a7fb/write-a-1000-word-seo-article-or-blog-post.jpg", IsPrimary = true, SortOrder = 0 }
                },
                Tags = new List<GigTag>
                {
                    new() { Tag = "blog writing" },
                    new() { Tag = "SEO" },
                    new() { Tag = "content writing" },
                    new() { Tag = "articles" }
                },
                Packages = new List<GigPackage>
                {
                    new()
                    {
                        PackageType = "basic",
                        Name = "Basic",
                        Price = 15.00m,
                        DeliveryDays = 2,
                        Revisions = "1",
                        Description = "500 word article",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "500 Words", SortOrder = 0 },
                            new() { FeatureText = "SEO Optimized", SortOrder = 1 },
                            new() { FeatureText = "1 Revision", SortOrder = 2 }
                        }
                    },
                    new()
                    {
                        PackageType = "standard",
                        Name = "Standard",
                        Price = 30.00m,
                        DeliveryDays = 3,
                        Revisions = "2",
                        Description = "1000 word article with keywords",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "1000 Words", SortOrder = 0 },
                            new() { FeatureText = "SEO Optimized", SortOrder = 1 },
                            new() { FeatureText = "Keyword Research", SortOrder = 2 },
                            new() { FeatureText = "2 Revisions", SortOrder = 3 }
                        }
                    },
                    new()
                    {
                        PackageType = "premium",
                        Name = "Premium",
                        Price = 60.00m,
                        DeliveryDays = 5,
                        Revisions = "Unlimited",
                        Description = "2000 word in-depth article",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "2000 Words", SortOrder = 0 },
                            new() { FeatureText = "SEO Optimized", SortOrder = 1 },
                            new() { FeatureText = "Keyword Research", SortOrder = 2 },
                            new() { FeatureText = "Meta Description", SortOrder = 3 },
                            new() { FeatureText = "Unlimited Revisions", SortOrder = 4 }
                        }
                    }
                }
            },

            // Omar's Gigs (Video)
            new()
            {
                Id = Guid.NewGuid(),
                SellerId = users[3].Id,
                CategoryId = videoCategory.Id,
                Title = "I will edit your YouTube videos professionally",
                Slug = "professional-youtube-video-editing",
                Description = "Transform your raw footage into engaging YouTube content! I provide professional video editing with color correction, transitions, sound design, and motion graphics. Let's make your channel stand out!",
                Rating = 4.7m,
                ReviewCount = 38,
                OrdersInQueue = 4,
                IsActive = true,
                Images = new List<GigImage>
                {
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/247979584/original/71b5f8a5a8f7b8a8f7b8a8f7b8a8f7b8a8f7b8a8/edit-your-youtube-videos-professionally.jpg", IsPrimary = true, SortOrder = 0 }
                },
                Tags = new List<GigTag>
                {
                    new() { Tag = "video editing" },
                    new() { Tag = "youtube" },
                    new() { Tag = "premiere pro" },
                    new() { Tag = "motion graphics" }
                },
                Packages = new List<GigPackage>
                {
                    new()
                    {
                        PackageType = "basic",
                        Name = "Basic",
                        Price = 35.00m,
                        DeliveryDays = 3,
                        Revisions = "2",
                        Description = "Up to 5 min video",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "Up to 5 Minutes", SortOrder = 0 },
                            new() { FeatureText = "Basic Cuts & Transitions", SortOrder = 1 },
                            new() { FeatureText = "Background Music", SortOrder = 2 }
                        }
                    },
                    new()
                    {
                        PackageType = "standard",
                        Name = "Standard",
                        Price = 75.00m,
                        DeliveryDays = 5,
                        Revisions = "3",
                        Description = "Up to 15 min video with effects",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "Up to 15 Minutes", SortOrder = 0 },
                            new() { FeatureText = "Color Correction", SortOrder = 1 },
                            new() { FeatureText = "Sound Design", SortOrder = 2 },
                            new() { FeatureText = "Text Animations", SortOrder = 3 }
                        }
                    },
                    new()
                    {
                        PackageType = "premium",
                        Name = "Premium",
                        Price = 150.00m,
                        DeliveryDays = 7,
                        Revisions = "Unlimited",
                        Description = "Full production video",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "Up to 30 Minutes", SortOrder = 0 },
                            new() { FeatureText = "Motion Graphics", SortOrder = 1 },
                            new() { FeatureText = "Custom Intro/Outro", SortOrder = 2 },
                            new() { FeatureText = "Thumbnail Design", SortOrder = 3 },
                            new() { FeatureText = "4K Export", SortOrder = 4 }
                        }
                    }
                }
            },

            // Lina's Gigs (Marketing)
            new()
            {
                Id = Guid.NewGuid(),
                SellerId = users[4].Id,
                CategoryId = marketingCategory.Id,
                Title = "I will manage your social media accounts and grow your audience",
                Slug = "social-media-management-growth",
                Description = "Struggling to grow your social media presence? I offer complete social media management including content creation, scheduling, engagement, and analytics. Let's build your brand together!",
                Rating = 4.9m,
                ReviewCount = 76,
                OrdersInQueue = 6,
                IsActive = true,
                Images = new List<GigImage>
                {
                    new() { ImageUrl = "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/157446845/original/e1f3f5a5f5a5f5a5f5a5f5a5f5a5f5a5f5a5f5a5/be-your-social-media-marketing-manager.jpg", IsPrimary = true, SortOrder = 0 }
                },
                Tags = new List<GigTag>
                {
                    new() { Tag = "social media" },
                    new() { Tag = "marketing" },
                    new() { Tag = "instagram" },
                    new() { Tag = "facebook" }
                },
                Packages = new List<GigPackage>
                {
                    new()
                    {
                        PackageType = "basic",
                        Name = "Basic",
                        Price = 50.00m,
                        DeliveryDays = 7,
                        Revisions = "2",
                        Description = "1 week management (1 platform)",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "1 Platform", SortOrder = 0 },
                            new() { FeatureText = "7 Posts", SortOrder = 1 },
                            new() { FeatureText = "Hashtag Research", SortOrder = 2 }
                        }
                    },
                    new()
                    {
                        PackageType = "standard",
                        Name = "Standard",
                        Price = 150.00m,
                        DeliveryDays = 30,
                        Revisions = "5",
                        Description = "1 month management (2 platforms)",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "2 Platforms", SortOrder = 0 },
                            new() { FeatureText = "30 Posts", SortOrder = 1 },
                            new() { FeatureText = "Engagement Strategy", SortOrder = 2 },
                            new() { FeatureText = "Monthly Report", SortOrder = 3 }
                        }
                    },
                    new()
                    {
                        PackageType = "premium",
                        Name = "Premium",
                        Price = 300.00m,
                        DeliveryDays = 30,
                        Revisions = "Unlimited",
                        Description = "Full social media management",
                        Features = new List<PackageFeature>
                        {
                            new() { FeatureText = "All Platforms", SortOrder = 0 },
                            new() { FeatureText = "60 Posts", SortOrder = 1 },
                            new() { FeatureText = "Paid Ad Management", SortOrder = 2 },
                            new() { FeatureText = "Influencer Outreach", SortOrder = 3 },
                            new() { FeatureText = "Weekly Reports", SortOrder = 4 }
                        }
                    }
                }
            }
        };

        await context.Gigs.AddRangeAsync(gigs);
        await context.SaveChangesAsync();

        // ============================================
        // SAMPLE REVIEWS
        // ============================================
        var reviews = new List<Review>();
        var reviewComments = new[]
        {
            "Excellent work! Exactly what I was looking for. Highly recommended!",
            "Very professional and delivered on time. Will definitely order again.",
            "Amazing quality and great communication throughout the project.",
            "Exceeded my expectations! Thank you so much.",
            "Great attention to detail. Very satisfied with the result."
        };

        var buyerNames = new[] { "John D.", "Emma S.", "Michael B.", "Lisa K.", "David R." };

        foreach (var gig in gigs.Take(3))
        {
            for (int i = 0; i < 3; i++)
            {
                // Create a dummy order first (required for review)
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}-{i}",
                    GigId = gig.Id,
                    BuyerId = null, // Anonymous buyer for seed data
                    SellerId = gig.SellerId,
                    PackageType = "basic",
                    Price = 25.00m,
                    ServiceFee = 2.50m,
                    TotalPrice = 27.50m,
                    Status = "completed",
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30))
                };
                await context.Orders.AddAsync(order);

                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    GigId = gig.Id,
                    SellerId = gig.SellerId,
                    Rating = Random.Shared.Next(4, 6),
                    Comment = reviewComments[Random.Shared.Next(reviewComments.Length)],
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30))
                };
                reviews.Add(review);
            }
        }

        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Database seeded successfully!");
        Console.WriteLine($"   - {categories.Count} categories");
        Console.WriteLine($"   - {users.Count} sellers");
        Console.WriteLine($"   - {gigs.Count} gigs");
        Console.WriteLine($"   - {reviews.Count} reviews");
    }
}

