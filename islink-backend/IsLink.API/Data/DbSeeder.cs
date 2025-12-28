using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsLink.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        Console.WriteLine("🌱 Starting database seeding process...");
        
        // Clear existing data to ensure fresh seed
        Console.WriteLine("🧹 Clearing existing seed data...");
        Console.WriteLine("!!! FORCE SEEDING ACTIVE - WIPING DATA !!!");

        // FIX: Ensure ChatSessions has Title column (Migration workaround for existing DBs)
        try 
        {
            // Simple check to avoid crashing if table doesn't exist yet (EnsureCreated handles creation, but this handles update)
            await context.Database.ExecuteSqlRawAsync(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'ChatSessions') THEN
                        ALTER TABLE ""ChatSessions"" ADD COLUMN IF NOT EXISTS ""Title"" text DEFAULT 'New Chat';
                        ALTER TABLE ""ChatSessions"" ADD COLUMN IF NOT EXISTS ""RecommendationsJson"" text;
                    END IF;
                END $$;
            ");
            Console.WriteLine("✅ Schema updated: Added Title & RecommendationsJson to ChatSessions");
        }
        catch (Exception ex)
        {
             Console.WriteLine($"⚠️ Schema update note (ignore if new DB): {ex.Message}");
        }
        
        // Clear in proper order to respect foreign keys
        if (await context.Reviews.AnyAsync())
        {
            context.Reviews.RemoveRange(await context.Reviews.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        if (await context.Orders.AnyAsync())
        {
            context.Orders.RemoveRange(await context.Orders.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        if (await context.Transactions.AnyAsync())
        {
            context.Transactions.RemoveRange(await context.Transactions.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        if (await context.Gigs.AnyAsync())
        {
            context.Gigs.RemoveRange(await context.Gigs.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        // Clear user skills and languages (cascade delete will handle this, but being explicit)
        if (await context.UserSkills.AnyAsync())
        {
            context.UserSkills.RemoveRange(await context.UserSkills.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        if (await context.UserLanguages.AnyAsync())
        {
            context.UserLanguages.RemoveRange(await context.UserLanguages.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        if (await context.Users.AnyAsync())
        {
            context.Users.RemoveRange(await context.Users.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        // Clear existing categories to re-seed properly
        if (await context.Categories.AnyAsync())
        {
            context.Categories.RemoveRange(await context.Categories.ToListAsync());
            await context.SaveChangesAsync();
        }
        
        Console.WriteLine("✅ Existing data cleared");

        Console.WriteLine("🌱 Seeding database with comprehensive test data...");

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
        // USERS (Mix of Sellers and Buyers)
        // ============================================
        var users = new List<User>
        {
            // === SELLERS ===
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
                Bio = "Professional graphic designer with 5+ years of experience. Specializing in logo design, brand identity, and UI/UX design. I've worked with clients from over 30 countries and delivered 500+ projects.",
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
                Bio = "Full-stack developer with expertise in React, Node.js, and .NET. Building scalable web applications for businesses worldwide. Former software engineer at a Fortune 500 company.",
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
                Bio = "Professional content writer and translator. Native Arabic speaker fluent in English and French. SEO-optimized content that converts. Published author with articles in major publications.",
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
                Bio = "Video editor and motion graphics artist. Creating stunning videos for YouTube, social media, and commercial projects. Worked with brands like Nike, Samsung, and local startups.",
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
                Bio = "Digital marketing expert specializing in social media management, SEO, and paid advertising. Helped 100+ businesses grow their online presence and increase revenue.",
                Rating = 4.9m,
                ReviewCount = 93,
                CompletedOrders = 134,
                IsOnline = true,
                IsVerified = true,
                Balance = 4100.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "khalil_music",
                Email = "khalil@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Khalil Nasser",
                AvatarUrl = "https://randomuser.me/api/portraits/men/52.jpg",
                Role = "seller",
                Country = "Egypt",
                Bio = "Music producer and audio engineer with 8 years of experience. Specializing in mixing, mastering, and voice-over production. Grammy-considered projects.",
                Rating = 4.8m,
                ReviewCount = 72,
                CompletedOrders = 95,
                IsOnline = false,
                IsVerified = true,
                Balance = 2800.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "nour_ai",
                Email = "nour@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Nour Ibrahim",
                AvatarUrl = "https://randomuser.me/api/portraits/women/33.jpg",
                Role = "seller",
                Country = "Syria",
                Bio = "AI & Machine Learning specialist. Building intelligent chatbots, automation systems, and data analysis solutions. PhD in Computer Science from MIT.",
                Rating = 5.0m,
                ReviewCount = 38,
                CompletedOrders = 42,
                IsOnline = true,
                IsVerified = true,
                Balance = 5200.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "rami_business",
                Email = "rami@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Rami Haddad",
                AvatarUrl = "https://randomuser.me/api/portraits/men/45.jpg",
                Role = "seller",
                Country = "Lebanon",
                Bio = "Business consultant and virtual assistant. Expert in business plans, financial modeling, and startup consulting. Former investment banker.",
                Rating = 4.6m,
                ReviewCount = 54,
                CompletedOrders = 67,
                IsOnline = true,
                IsVerified = false,
                Balance = 1500.00m
            },
            // === BUYERS ===
            new()
            {
                Id = Guid.NewGuid(),
                Username = "john_startup",
                Email = "john@startup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "John Miller",
                AvatarUrl = "https://randomuser.me/api/portraits/men/12.jpg",
                Role = "buyer",
                Country = "USA",
                Bio = "Tech startup founder looking for talented freelancers to help build my vision.",
                IsOnline = true,
                IsVerified = true,
                Balance = 500.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "emma_brand",
                Email = "emma@brandco.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Emma Thompson",
                AvatarUrl = "https://randomuser.me/api/portraits/women/15.jpg",
                Role = "buyer",
                Country = "UK",
                Bio = "Marketing manager at a growing e-commerce company. Always looking for creative talent.",
                IsOnline = false,
                IsVerified = true,
                Balance = 1200.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "michael_agency",
                Email = "michael@agency.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Michael Chen",
                AvatarUrl = "https://randomuser.me/api/portraits/men/22.jpg",
                Role = "buyer",
                Country = "Canada",
                Bio = "Creative agency owner outsourcing specialized projects to top freelancers.",
                IsOnline = true,
                IsVerified = true,
                Balance = 3000.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "lisa_restaurant",
                Email = "lisa@restaurant.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Lisa Rodriguez",
                AvatarUrl = "https://randomuser.me/api/portraits/women/28.jpg",
                Role = "buyer",
                Country = "Spain",
                Bio = "Restaurant owner looking for help with branding and social media.",
                IsOnline = false,
                IsVerified = false,
                Balance = 200.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "david_youtuber",
                Email = "david@youtube.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "David Kim",
                AvatarUrl = "https://randomuser.me/api/portraits/men/35.jpg",
                Role = "buyer",
                Country = "South Korea",
                Bio = "YouTube content creator with 500K subscribers. Need video editing and thumbnails.",
                IsOnline = true,
                IsVerified = true,
                Balance = 800.00m
            },
            // === ADDITIONAL SELLERS ===
            new()
            {
                Id = Guid.NewGuid(),
                Username = "zain_webdev",
                Email = "zain@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Zain Al-Din",
                AvatarUrl = "https://randomuser.me/api/portraits/men/28.jpg",
                Role = "seller",
                Country = "Syria",
                Bio = "Expert WordPress developer specializing in custom themes, plugins, and WooCommerce stores. 6+ years experience building responsive websites.",
                Rating = 4.8m,
                ReviewCount = 156,
                CompletedOrders = 198,
                IsOnline = true,
                IsVerified = true,
                Balance = 3800.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "layla_photo",
                Email = "layla@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Layla Mahmoud",
                AvatarUrl = "https://randomuser.me/api/portraits/women/41.jpg",
                Role = "seller",
                Country = "Jordan",
                Bio = "Professional photographer and photo editor. Specializing in product photography, portrait retouching, and lifestyle photography.",
                Rating = 4.9m,
                ReviewCount = 112,
                CompletedOrders = 145,
                IsOnline = false,
                IsVerified = true,
                Balance = 2200.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "youssef_mobile",
                Email = "youssef@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Youssef Ali",
                AvatarUrl = "https://randomuser.me/api/portraits/men/61.jpg",
                Role = "seller",
                Country = "Lebanon",
                Bio = "Mobile app developer for iOS and Android. Expert in React Native, Flutter, and native development. Published 20+ apps on app stores.",
                Rating = 5.0m,
                ReviewCount = 87,
                CompletedOrders = 103,
                IsOnline = true,
                IsVerified = true,
                Balance = 4500.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "fatima_voice",
                Email = "fatima@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Fatima Hussein",
                AvatarUrl = "https://randomuser.me/api/portraits/women/56.jpg",
                Role = "seller",
                Country = "Iraq",
                Bio = "Professional voice-over artist in Arabic and English. Female voice specialist for commercials, audiobooks, e-learning, and narration.",
                Rating = 4.9m,
                ReviewCount = 203,
                CompletedOrders = 267,
                IsOnline = true,
                IsVerified = true,
                Balance = 3100.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "karim_3d",
                Email = "karim@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Karim Saleh",
                AvatarUrl = "https://randomuser.me/api/portraits/men/38.jpg",
                Role = "seller",
                Country = "Egypt",
                Bio = "3D artist and animator. Creating stunning 3D models, animations, and visualizations for games, architecture, and marketing.",
                Rating = 4.7m,
                ReviewCount = 94,
                CompletedOrders = 118,
                IsOnline = false,
                IsVerified = true,
                Balance = 2700.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "hala_data",
                Email = "hala@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Hala Taha",
                AvatarUrl = "https://randomuser.me/api/portraits/women/63.jpg",
                Role = "seller",
                Country = "Syria",
                Bio = "Data analyst and visualization expert. Transforming raw data into actionable insights using Python, SQL, Tableau, and Power BI.",
                Rating = 4.8m,
                ReviewCount = 71,
                CompletedOrders = 89,
                IsOnline = true,
                IsVerified = true,
                Balance = 1900.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "bassam_uiux",
                Email = "bassam@islink.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Bassam Khoury",
                AvatarUrl = "https://randomuser.me/api/portraits/men/48.jpg",
                Role = "seller",
                Country = "Lebanon",
                Bio = "UI/UX designer creating user-centered designs for web and mobile apps. 7+ years experience with Figma, Sketch, and prototyping.",
                Rating = 4.9m,
                ReviewCount = 128,
                CompletedOrders = 167,
                IsOnline = true,
                IsVerified = true,
                Balance = 3600.00m
            },
            // === ADDITIONAL BUYERS ===
            new()
            {
                Id = Guid.NewGuid(),
                Username = "sophia_ecommerce",
                Email = "sophia@shop.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Sophia Williams",
                AvatarUrl = "https://randomuser.me/api/portraits/women/45.jpg",
                Role = "buyer",
                Country = "Australia",
                Bio = "E-commerce store owner looking for professional services to grow my business.",
                IsOnline = false,
                IsVerified = true,
                Balance = 1500.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "james_startup",
                Email = "james@startup.io",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "James Wilson",
                AvatarUrl = "https://randomuser.me/api/portraits/men/56.jpg",
                Role = "buyer",
                Country = "USA",
                Bio = "Startup founder seeking talented freelancers to help build innovative products.",
                IsOnline = true,
                IsVerified = true,
                Balance = 2500.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "maria_agency",
                Email = "maria@agency.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Maria Garcia",
                AvatarUrl = "https://randomuser.me/api/portraits/women/51.jpg",
                Role = "buyer",
                Country = "Mexico",
                Bio = "Marketing agency owner looking for creative professionals.",
                IsOnline = true,
                IsVerified = false,
                Balance = 900.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "ali_entrepreneur",
                Email = "ali@business.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Ali Al-Mansouri",
                AvatarUrl = "https://randomuser.me/api/portraits/men/71.jpg",
                Role = "buyer",
                Country = "Saudi Arabia",
                Bio = "Entrepreneur building multiple online businesses. Always looking for quality services.",
                IsOnline = false,
                IsVerified = true,
                Balance = 4200.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "luna_creator",
                Email = "luna@content.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Luna Park",
                AvatarUrl = "https://randomuser.me/api/portraits/women/58.jpg",
                Role = "buyer",
                Country = "Japan",
                Bio = "Content creator and influencer needing professional design and video services.",
                IsOnline = true,
                IsVerified = true,
                Balance = 1100.00m
            }
        };

        // Add skills to sellers
        users[0].Skills = new List<UserSkill>
        {
            new() { SkillName = "Logo Design" },
            new() { SkillName = "Brand Identity" },
            new() { SkillName = "UI/UX Design" },
            new() { SkillName = "Adobe Photoshop" },
            new() { SkillName = "Adobe Illustrator" },
            new() { SkillName = "Figma" }
        };

        users[1].Skills = new List<UserSkill>
        {
            new() { SkillName = "React" },
            new() { SkillName = "Node.js" },
            new() { SkillName = ".NET" },
            new() { SkillName = "PostgreSQL" },
            new() { SkillName = "MongoDB" },
            new() { SkillName = "TypeScript" }
        };

        users[2].Skills = new List<UserSkill>
        {
            new() { SkillName = "Content Writing" },
            new() { SkillName = "Translation" },
            new() { SkillName = "SEO Writing" },
            new() { SkillName = "Copywriting" },
            new() { SkillName = "Arabic" },
            new() { SkillName = "French" }
        };

        users[3].Skills = new List<UserSkill>
        {
            new() { SkillName = "Video Editing" },
            new() { SkillName = "Motion Graphics" },
            new() { SkillName = "After Effects" },
            new() { SkillName = "Premiere Pro" },
            new() { SkillName = "DaVinci Resolve" }
        };

        users[4].Skills = new List<UserSkill>
        {
            new() { SkillName = "Social Media Marketing" },
            new() { SkillName = "SEO" },
            new() { SkillName = "Google Ads" },
            new() { SkillName = "Facebook Ads" },
            new() { SkillName = "Content Strategy" }
        };

        users[5].Skills = new List<UserSkill>
        {
            new() { SkillName = "Music Production" },
            new() { SkillName = "Mixing" },
            new() { SkillName = "Mastering" },
            new() { SkillName = "Voice Over" },
            new() { SkillName = "Sound Design" }
        };

        users[6].Skills = new List<UserSkill>
        {
            new() { SkillName = "Machine Learning" },
            new() { SkillName = "Python" },
            new() { SkillName = "TensorFlow" },
            new() { SkillName = "ChatGPT Integration" },
            new() { SkillName = "Data Analysis" }
        };

        users[7].Skills = new List<UserSkill>
        {
            new() { SkillName = "Business Plans" },
            new() { SkillName = "Financial Modeling" },
            new() { SkillName = "Virtual Assistance" },
            new() { SkillName = "Data Entry" },
            new() { SkillName = "Excel" }
        };

        // Skills for new sellers (indices 8-14)
        users[8].Skills = new List<UserSkill>
        {
            new() { SkillName = "WordPress" },
            new() { SkillName = "PHP" },
            new() { SkillName = "WooCommerce" },
            new() { SkillName = "CSS" },
            new() { SkillName = "JavaScript" }
        };

        users[9].Skills = new List<UserSkill>
        {
            new() { SkillName = "Photography" },
            new() { SkillName = "Photo Editing" },
            new() { SkillName = "Adobe Photoshop" },
            new() { SkillName = "Lightroom" },
            new() { SkillName = "Product Photography" }
        };

        users[10].Skills = new List<UserSkill>
        {
            new() { SkillName = "React Native" },
            new() { SkillName = "Flutter" },
            new() { SkillName = "iOS Development" },
            new() { SkillName = "Android Development" },
            new() { SkillName = "App Store Optimization" }
        };

        users[11].Skills = new List<UserSkill>
        {
            new() { SkillName = "Voice Over" },
            new() { SkillName = "Arabic Voice" },
            new() { SkillName = "English Voice" },
            new() { SkillName = "Commercial" },
            new() { SkillName = "Audiobook Narration" }
        };

        users[12].Skills = new List<UserSkill>
        {
            new() { SkillName = "3D Modeling" },
            new() { SkillName = "Blender" },
            new() { SkillName = "Maya" },
            new() { SkillName = "3D Animation" },
            new() { SkillName = "Texturing" }
        };

        users[13].Skills = new List<UserSkill>
        {
            new() { SkillName = "Data Analysis" },
            new() { SkillName = "Python" },
            new() { SkillName = "SQL" },
            new() { SkillName = "Tableau" },
            new() { SkillName = "Power BI" }
        };

        users[14].Skills = new List<UserSkill>
        {
            new() { SkillName = "UI Design" },
            new() { SkillName = "UX Design" },
            new() { SkillName = "Figma" },
            new() { SkillName = "Sketch" },
            new() { SkillName = "Prototyping" }
        };

        // Add languages to all sellers (first 15 are sellers)
        foreach (var user in users.Take(15))
        {
            user.Languages = new List<UserLanguage>
            {
                new() { LanguageName = "Arabic", Proficiency = "Native" },
                new() { LanguageName = "English", Proficiency = "Fluent" }
            };
        }
        
        // Add languages to buyers (indices 15+)
        users[15].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" } };
        users[16].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" } };
        users[17].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" }, new() { LanguageName = "Mandarin", Proficiency = "Fluent" } };
        users[18].Languages = new List<UserLanguage> { new() { LanguageName = "Spanish", Proficiency = "Native" }, new() { LanguageName = "English", Proficiency = "Fluent" } };
        users[19].Languages = new List<UserLanguage> { new() { LanguageName = "Korean", Proficiency = "Native" }, new() { LanguageName = "English", Proficiency = "Fluent" } };
        users[20].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" } };
        users[21].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" } };
        users[22].Languages = new List<UserLanguage> { new() { LanguageName = "Spanish", Proficiency = "Native" }, new() { LanguageName = "English", Proficiency = "Fluent" } };
        users[23].Languages = new List<UserLanguage> { new() { LanguageName = "Arabic", Proficiency = "Native" }, new() { LanguageName = "English", Proficiency = "Fluent" } };
        users[24].Languages = new List<UserLanguage> { new() { LanguageName = "Japanese", Proficiency = "Native" }, new() { LanguageName = "English", Proficiency = "Fluent" } };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Get category references
        var graphicsCategory = categories.First(c => c.Slug == "graphics-design");
        var programmingCategory = categories.First(c => c.Slug == "programming-tech");
        var writingCategory = categories.First(c => c.Slug == "writing-translation");
        var videoCategory = categories.First(c => c.Slug == "video-animation");
        var marketingCategory = categories.First(c => c.Slug == "digital-marketing");
        var musicCategory = categories.First(c => c.Slug == "music-audio");
        var aiCategory = categories.First(c => c.Slug == "ai-services");
        var businessCategory = categories.First(c => c.Slug == "business");

        // ============================================
        // GIGS (Multiple per seller)
        // ============================================
        var gigs = new List<Gig>
        {
            // === Sarah's Gigs (Designer) ===
            CreateGig(
                users[0].Id, graphicsCategory.Id,
                "I will design a modern minimalist logo for your business",
                "modern-minimalist-logo-design",
                "Looking for a professional logo that represents your brand? I create modern, minimalist logos that are memorable and versatile. Each logo is crafted with attention to detail and designed to work across all platforms - from business cards to billboards. With over 500 logos delivered, I know how to capture your brand's essence.",
                4.9m, 127, 5,
                new[] { "https://images.unsplash.com/photo-1626785774573-4b799314346d?w=800&h=600&fit=crop" },
                new[] { "logo design", "minimalist", "branding", "modern" },
                25, 50, 100, 3, 5, 7
            ),
            CreateGig(
                users[0].Id, graphicsCategory.Id,
                "I will create a complete brand identity package",
                "complete-brand-identity-package",
                "Get everything your brand needs to stand out! This package includes logo, business cards, letterhead, social media kit, and brand guidelines. Perfect for new businesses or rebrands looking for a cohesive visual identity.",
                4.9m, 45, 3,
                new[] { "https://images.unsplash.com/photo-1634942537034-2531766767d1?w=800&h=600&fit=crop" },
                new[] { "brand identity", "branding", "business cards", "logo" },
                150, 300, 500, 7, 10, 14
            ),
            CreateGig(
                users[0].Id, graphicsCategory.Id,
                "I will design stunning social media graphics",
                "stunning-social-media-graphics",
                "Boost your social media presence with eye-catching graphics! I create posts, stories, covers, and ads for Instagram, Facebook, LinkedIn, and more. Consistent branding that gets engagement.",
                4.8m, 38, 8,
                new[] { "https://images.unsplash.com/photo-1611162617474-5b21e879e113?w=800&h=600&fit=crop" },
                new[] { "social media", "instagram", "facebook", "graphics" },
                20, 45, 80, 2, 3, 5
            ),

            // === Ahmed's Gigs (Developer) ===
            CreateGig(
                users[1].Id, programmingCategory.Id,
                "I will build a responsive website using React and Node.js",
                "responsive-react-nodejs-website",
                "Need a modern, fast, and responsive website? I specialize in building full-stack web applications using React for the frontend and Node.js for the backend. Clean code, best practices, and SEO-friendly structure included. Deployed on your preferred hosting.",
                5.0m, 89, 3,
                new[] { "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=800&h=600&fit=crop" },
                new[] { "react", "nodejs", "web development", "responsive" },
                100, 250, 500, 7, 14, 21
            ),
            CreateGig(
                users[1].Id, programmingCategory.Id,
                "I will develop a custom e-commerce website",
                "custom-ecommerce-website",
                "Launch your online store with a custom e-commerce solution! Built with modern technologies, including shopping cart, payment integration (Stripe/PayPal), inventory management, and admin dashboard. Fully responsive and SEO-optimized.",
                5.0m, 34, 2,
                new[] { "https://images.unsplash.com/photo-1556742049-0cfed4f7a07d?w=800&h=600&fit=crop" },
                new[] { "ecommerce", "online store", "stripe", "shopify" },
                300, 600, 1000, 14, 21, 30
            ),
            CreateGig(
                users[1].Id, programmingCategory.Id,
                "I will fix bugs and optimize your website",
                "fix-bugs-optimize-website",
                "Having issues with your website? I'll fix bugs, improve performance, and optimize your code. Experienced with React, Angular, Vue, Node.js, PHP, and more. Fast turnaround with detailed documentation.",
                4.9m, 56, 6,
                new[] { "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800&h=600&fit=crop" },
                new[] { "bug fix", "optimization", "debugging", "performance" },
                30, 75, 150, 1, 3, 5
            ),

            // === Maya's Gigs (Writer) ===
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will write SEO optimized blog posts and articles",
                "seo-optimized-blog-posts-articles",
                "Get high-quality, SEO-optimized content that ranks! I write engaging blog posts and articles that drive traffic to your website. Native Arabic speaker with excellent English skills. Niche expertise in tech, business, and lifestyle.",
                4.8m, 64, 8,
                new[] { "https://images.unsplash.com/photo-1455390582262-044cdead277a?w=800&h=600&fit=crop" },
                new[] { "blog writing", "SEO", "content writing", "articles" },
                15, 30, 60, 2, 3, 5
            ),
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will translate documents from Arabic to English",
                "arabic-english-translation",
                "Professional Arabic-English translation services. Accurate, culturally-sensitive translations for business documents, marketing materials, legal papers, and more. Native Arabic speaker with 10+ years of translation experience.",
                4.9m, 41, 4,
                new[] { "https://images.unsplash.com/photo-1456513080510-7bf3a84b82f8?w=800&h=600&fit=crop" },
                new[] { "translation", "arabic", "english", "documents" },
                20, 50, 100, 2, 4, 7
            ),
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will write compelling website copy",
                "compelling-website-copy",
                "Your website deserves words that convert! I write persuasive website copy that engages visitors and drives action. From landing pages to About Us sections, I'll craft copy that reflects your brand voice.",
                4.7m, 28, 5,
                new[] { "https://images.unsplash.com/photo-1519337265831-281ec6cc8514?w=800&h=600&fit=crop" },
                new[] { "copywriting", "website copy", "landing page", "conversion" },
                40, 80, 150, 3, 5, 7
            ),

            // === Omar's Gigs (Video) ===
            CreateGig(
                users[3].Id, videoCategory.Id,
                "I will edit your YouTube videos professionally",
                "professional-youtube-video-editing",
                "Transform your raw footage into engaging YouTube content! I provide professional video editing with color correction, transitions, sound design, and motion graphics. Let's make your channel stand out and grow!",
                4.7m, 45, 4,
                new[] { "https://images.unsplash.com/photo-1574717432729-24e53b879425?w=800&h=600&fit=crop" },
                new[] { "video editing", "youtube", "premiere pro", "motion graphics" },
                35, 75, 150, 3, 5, 7
            ),
            CreateGig(
                users[3].Id, videoCategory.Id,
                "I will create animated explainer videos",
                "animated-explainer-videos",
                "Explain your product or service with an engaging animated video! Perfect for startups, apps, and services. Includes script writing, voice-over coordination, and unlimited revisions.",
                4.8m, 22, 2,
                new[] { "https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800&h=600&fit=crop" },
                new[] { "animation", "explainer video", "after effects", "motion graphics" },
                100, 200, 400, 7, 10, 14
            ),

            // === Lina's Gigs (Marketing) ===
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will manage your social media accounts and grow your audience",
                "social-media-management-growth",
                "Struggling to grow your social media presence? I offer complete social media management including content creation, scheduling, engagement, and analytics. Let's build your brand together!",
                4.9m, 93, 6,
                new[] { "https://images.unsplash.com/photo-1611926653458-09294b3142bf?w=800&h=600&fit=crop" },
                new[] { "social media", "marketing", "instagram", "facebook" },
                50, 150, 300, 7, 30, 30
            ),
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will run Facebook and Instagram ads campaigns",
                "facebook-instagram-ads-campaigns",
                "Get more leads and sales with targeted Facebook and Instagram ads! I'll create, manage, and optimize your ad campaigns for maximum ROI. Includes audience research, ad copy, and detailed reporting.",
                4.8m, 67, 4,
                new[] { "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=800&h=600&fit=crop" },
                new[] { "facebook ads", "instagram ads", "PPC", "advertising" },
                100, 250, 500, 7, 14, 30
            ),
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will do complete SEO optimization for your website",
                "complete-seo-optimization",
                "Rank higher on Google with comprehensive SEO! Includes keyword research, on-page optimization, technical SEO audit, backlink strategy, and monthly progress reports. White-hat techniques only.",
                4.9m, 48, 3,
                new[] { "https://images.unsplash.com/photo-1571786256017-aee7a0c009b6?w=800&h=600&fit=crop" },
                new[] { "SEO", "google ranking", "keywords", "optimization" },
                75, 200, 400, 7, 14, 30
            ),

            // === Khalil's Gigs (Music) ===
            CreateGig(
                users[5].Id, musicCategory.Id,
                "I will mix and master your song professionally",
                "professional-mixing-mastering",
                "Get radio-ready sound with professional mixing and mastering! 8 years of experience in the music industry. Your track will sound polished, balanced, and ready for release on all platforms.",
                4.8m, 72, 5,
                new[] { "https://images.unsplash.com/photo-1598488035139-bdbb2231ce04?w=800&h=600&fit=crop" },
                new[] { "mixing", "mastering", "music production", "audio" },
                50, 100, 200, 3, 5, 7
            ),
            CreateGig(
                users[5].Id, musicCategory.Id,
                "I will record a professional voice over",
                "professional-voice-over",
                "Need a professional voice for your project? I offer voice-over services in Arabic and English for commercials, explainer videos, audiobooks, and more. Fast delivery with unlimited revisions.",
                4.7m, 35, 7,
                new[] { "https://images.unsplash.com/photo-1590602847861-f357a9332bbc?w=800&h=600&fit=crop" },
                new[] { "voice over", "narration", "commercial", "audiobook" },
                25, 50, 100, 1, 2, 3
            ),

            // === Nour's Gigs (AI) ===
            CreateGig(
                users[6].Id, aiCategory.Id,
                "I will build a custom AI chatbot for your business",
                "custom-ai-chatbot-business",
                "Automate customer support with an intelligent AI chatbot! I build custom chatbots using GPT-4, integrated with your website or app. Trained on your business data for accurate responses.",
                5.0m, 38, 2,
                new[] { "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=800&h=600&fit=crop" },
                new[] { "AI chatbot", "GPT", "automation", "customer support" },
                200, 500, 1000, 7, 14, 21
            ),
            CreateGig(
                users[6].Id, aiCategory.Id,
                "I will automate your workflows with AI",
                "automate-workflows-ai",
                "Save time and reduce errors with AI automation! I'll analyze your workflows and implement AI solutions using Python, APIs, and machine learning. From data processing to report generation.",
                5.0m, 24, 1,
                new[] { "https://images.unsplash.com/photo-1555255707-c07966088b7b?w=800&h=600&fit=crop" },
                new[] { "AI automation", "python", "machine learning", "workflow" },
                150, 350, 700, 5, 10, 14
            ),

            // === Rami's Gigs (Business) ===
            CreateGig(
                users[7].Id, businessCategory.Id,
                "I will create a professional business plan",
                "professional-business-plan",
                "Need funding or strategic direction? I create comprehensive business plans including market analysis, financial projections, and growth strategies. Perfect for startups and established businesses.",
                4.6m, 54, 3,
                new[] { "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800&h=600&fit=crop" },
                new[] { "business plan", "startup", "financial projections", "strategy" },
                100, 250, 500, 5, 7, 14
            ),
            CreateGig(
                users[7].Id, businessCategory.Id,
                "I will be your virtual assistant for a week",
                "virtual-assistant-week",
                "Need help managing your workload? I provide professional virtual assistance including email management, scheduling, research, data entry, and customer support. Let me handle the busy work!",
                4.5m, 28, 4,
                new[] { "https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?w=800&h=600&fit=crop" },
                new[] { "virtual assistant", "admin support", "data entry", "scheduling" },
                50, 100, 200, 7, 7, 7
            ),

            // === Zain's Gigs (WordPress Developer) ===
            CreateGig(
                users[8].Id, programmingCategory.Id,
                "I will build a custom WordPress website",
                "custom-wordpress-website",
                "Get a professional WordPress website tailored to your needs! Custom theme development, plugin integration, WooCommerce setup, and full responsive design. SEO optimized and fast loading.",
                4.8m, 156, 5,
                new[] { "https://images.unsplash.com/photo-1616469829941-c7200edec809?w=800&h=600&fit=crop" },
                new[] { "wordpress", "website", "woocommerce", "custom theme" },
                80, 200, 400, 5, 10, 14
            ),
            CreateGig(
                users[8].Id, programmingCategory.Id,
                "I will create a WooCommerce store for you",
                "woocommerce-store-setup",
                "Launch your online store with WooCommerce! Complete setup including product configuration, payment gateways, shipping options, and store customization. Ready to sell!",
                4.9m, 89, 3,
                new[] { "https://images.unsplash.com/photo-1556742111-a301076d9d18?w=800&h=600&fit=crop" },
                new[] { "woocommerce", "ecommerce", "online store", "shopify alternative" },
                150, 350, 600, 7, 14, 21
            ),

            // === Layla's Gigs (Photographer) ===
            CreateGig(
                users[9].Id, graphicsCategory.Id,
                "I will edit your photos professionally",
                "professional-photo-editing",
                "Transform your photos with professional editing! Color correction, retouching, background removal, and enhancement. Perfect for portraits, products, and social media content.",
                4.9m, 112, 6,
                new[] { "https://images.unsplash.com/photo-1550948956-65b828882ae7?w=800&h=600&fit=crop" },
                new[] { "photo editing", "photoshop", "retouching", "color correction" },
                15, 35, 70, 1, 2, 3
            ),
            CreateGig(
                users[9].Id, graphicsCategory.Id,
                "I will create product photos with white background",
                "product-photos-white-background",
                "Professional product photography with clean white backgrounds. Perfect for e-commerce stores, Amazon listings, and catalogs. High-resolution images ready to use.",
                4.8m, 67, 4,
                new[] { "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=800&h=600&fit=crop" },
                new[] { "product photography", "white background", "ecommerce", "amazon" },
                20, 50, 100, 2, 3, 5
            ),

            // === Youssef's Gigs (Mobile Developer) ===
            CreateGig(
                users[10].Id, programmingCategory.Id,
                "I will develop a mobile app for iOS and Android",
                "ios-android-mobile-app",
                "Build your app once, deploy everywhere! React Native mobile app development for both iOS and Android. Native performance with cross-platform efficiency.",
                5.0m, 87, 2,
                new[] { "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800&h=600&fit=crop" },
                new[] { "react native", "mobile app", "ios", "android" },
                200, 500, 1000, 14, 21, 30
            ),
            CreateGig(
                users[10].Id, programmingCategory.Id,
                "I will fix bugs in your mobile app",
                "fix-mobile-app-bugs",
                "Having issues with your mobile app? I'll debug, fix crashes, optimize performance, and ensure smooth operation on both iOS and Android platforms.",
                4.9m, 45, 5,
                new[] { "https://images.unsplash.com/photo-1551650975-87deedd944c3?w=800&h=600&fit=crop" },
                new[] { "bug fix", "mobile app", "debugging", "optimization" },
                40, 100, 200, 2, 5, 7
            ),

            // === Fatima's Gigs (Voice Over) ===
            CreateGig(
                users[11].Id, musicCategory.Id,
                "I will record professional Arabic voice over",
                "professional-arabic-voice-over",
                "Native Arabic speaker providing professional voice-over services. Perfect for commercials, explainer videos, e-learning, and narration. Warm, clear, and engaging voice.",
                4.9m, 203, 7,
                new[] { "https://images.unsplash.com/photo-1478737270239-2f02b77ac6d5?w=800&h=600&fit=crop" },
                new[] { "voice over", "arabic", "commercial", "narration" },
                30, 70, 140, 1, 2, 3
            ),
            CreateGig(
                users[11].Id, musicCategory.Id,
                "I will narrate your audiobook in Arabic or English",
                "audiobook-narration-arabic-english",
                "Bring your book to life with professional audiobook narration! Experienced narrator with recording studio quality. Available in both Arabic and English.",
                5.0m, 56, 2,
                new[] { "https://images.unsplash.com/photo-1519791883288-dc8bd696e667?w=800&h=600&fit=crop" },
                new[] { "audiobook", "narration", "voice over", "book" },
                100, 250, 500, 14, 21, 30
            ),

            // === Karim's Gigs (3D Artist) ===
            CreateGig(
                users[12].Id, videoCategory.Id,
                "I will create 3D models for games or products",
                "3d-models-games-products",
                "High-quality 3D models for games, product visualization, or marketing. Low-poly or high-detail models with textures and materials. Ready for your project!",
                4.7m, 94, 4,
                new[] { "https://images.unsplash.com/photo-1617791160505-6f00504e3519?w=800&h=600&fit=crop" },
                new[] { "3d modeling", "blender", "maya", "3d models" },
                50, 120, 250, 3, 5, 7
            ),
            CreateGig(
                users[12].Id, videoCategory.Id,
                "I will create 3D product animations",
                "3d-product-animations",
                "Showcase your product with stunning 3D animations! Perfect for marketing, commercials, and social media. Professional quality that grabs attention.",
                4.8m, 38, 3,
                new[] { "https://images.unsplash.com/photo-1633412802994-5c058f151b66?w=800&h=600&fit=crop" },
                new[] { "3d animation", "product animation", "commercial", "motion graphics" },
                80, 180, 350, 5, 7, 10
            ),

            // === Hala's Gigs (Data Analyst) ===
            CreateGig(
                users[13].Id, businessCategory.Id,
                "I will analyze your data and create visualizations",
                "data-analysis-visualizations",
                "Turn your raw data into actionable insights! Data analysis, statistical modeling, and beautiful visualizations using Python, Tableau, and Power BI. Detailed reports included.",
                4.8m, 71, 3,
                new[] { "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&h=600&fit=crop" },
                new[] { "data analysis", "python", "tableau", "visualization" },
                60, 150, 300, 3, 5, 7
            ),
            CreateGig(
                users[13].Id, businessCategory.Id,
                "I will create interactive dashboards in Tableau or Power BI",
                "interactive-dashboards-tableau-powerbi",
                "Transform your business data into interactive dashboards! Real-time insights, beautiful visualizations, and user-friendly interfaces. Perfect for executives and teams.",
                4.9m, 42, 2,
                new[] { "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800&h=600&fit=crop" },
                new[] { "tableau", "power bi", "dashboard", "business intelligence" },
                100, 250, 500, 5, 7, 10
            ),

            // === Bassam's Gigs (UI/UX Designer) ===
            CreateGig(
                users[14].Id, graphicsCategory.Id,
                "I will design a modern UI/UX for your app or website",
                "modern-ui-ux-design",
                "User-centered design that converts! Complete UI/UX design including wireframes, mockups, prototypes, and design systems. Figma files included with handoff documentation.",
                4.9m, 128, 4,
                new[] { "https://images.unsplash.com/photo-1586717791821-3f44a5638d0f?w=800&h=600&fit=crop" },
                new[] { "ui design", "ux design", "figma", "prototype" },
                100, 250, 500, 5, 10, 14
            ),
            CreateGig(
                users[14].Id, graphicsCategory.Id,
                "I will redesign your existing website or app",
                "website-app-redesign",
                "Modernize your digital presence with a complete redesign! Improved user experience, updated visuals, and better conversion rates. Based on UX best practices.",
                4.8m, 73, 5,
                new[] { "https://images.unsplash.com/photo-1581291518633-83b4ebd1d83e?w=800&h=600&fit=crop" },
                new[] { "redesign", "ui ux", "modern design", "conversion optimization" },
                150, 350, 700, 7, 14, 21
            ),

            // === ADDITIONAL GIGS FOR EACH CATEGORY ===
            
            // More Graphics & Design Gigs
            CreateGig(
                users[0].Id, graphicsCategory.Id,
                "I will create custom illustrations and digital artwork",
                "custom-illustrations-digital-artwork",
                "Unique illustrations and digital artwork for your projects! Character design, book illustrations, concept art, and custom graphics. Available in various styles from cartoon to realistic.",
                4.8m, 92, 6,
                new[] { "https://images.unsplash.com/photo-1626785774573-4b799314346d?w=800&h=600&fit=crop" },
                new[] { "illustration", "digital art", "character design", "artwork" },
                40, 90, 180, 3, 5, 7
            ),
            CreateGig(
                users[9].Id, graphicsCategory.Id,
                "I will design custom flyers and posters",
                "custom-flyers-posters-design",
                "Eye-catching flyers and posters for your events, promotions, or business! Professional design with print-ready files. Fast turnaround and unlimited revisions.",
                4.7m, 156, 8,
                new[] { "https://images.unsplash.com/photo-1558655146-d09347e92766?w=800&h=600&fit=crop" },
                new[] { "flyer design", "poster design", "print design", "marketing materials" },
                20, 45, 90, 2, 3, 5
            ),
            CreateGig(
                users[9].Id, graphicsCategory.Id,
                "I will design professional business cards",
                "professional-business-cards-design",
                "Stand out with professionally designed business cards! Modern and elegant designs that represent your brand. Print-ready files included. Multiple design concepts.",
                4.9m, 203, 5,
                new[] { "https://images.unsplash.com/photo-1634942537034-2531766767d1?w=800&h=600&fit=crop" },
                new[] { "business cards", "card design", "print design", "branding" },
                15, 35, 70, 1, 2, 3
            ),

            // More Programming & Tech Gigs
            CreateGig(
                users[1].Id, programmingCategory.Id,
                "I will create a RESTful API using .NET Core",
                "restful-api-dotnet-core",
                "Build robust REST APIs with .NET Core! Clean architecture, authentication, documentation, and testing included. Scalable and production-ready code.",
                5.0m, 67, 3,
                new[] { "https://images.unsplash.com/photo-1605379399642-870262d3d051?w=800&h=600&fit=crop" },
                new[] { "api", ".net core", "rest api", "backend" },
                150, 350, 700, 5, 10, 14
            ),
            CreateGig(
                users[8].Id, programmingCategory.Id,
                "I will customize your WordPress theme",
                "wordpress-theme-customization",
                "Make your WordPress site unique with custom theme modifications! CSS tweaks, functionality additions, and layout changes. Responsive design ensured.",
                4.8m, 124, 7,
                new[] { "https://images.unsplash.com/photo-1616469829941-c7200edec809?w=800&h=600&fit=crop" },
                new[] { "wordpress", "theme customization", "php", "css" },
                30, 75, 150, 2, 4, 7
            ),
            CreateGig(
                users[10].Id, programmingCategory.Id,
                "I will convert your website to a mobile app",
                "website-to-mobile-app-conversion",
                "Transform your website into a native mobile app! React Native or Flutter. App store submission support included. Cross-platform iOS and Android.",
                4.9m, 56, 2,
                new[] { "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800&h=600&fit=crop" },
                new[] { "mobile app", "react native", "flutter", "ios android" },
                200, 500, 1000, 10, 14, 21
            ),
            CreateGig(
                users[1].Id, programmingCategory.Id,
                "I will set up CI/CD pipeline for your project",
                "cicd-pipeline-setup",
                "Automate your deployment with CI/CD pipelines! GitHub Actions, GitLab CI, or Jenkins. Automated testing, building, and deployment. Documentation included.",
                4.9m, 38, 1,
                new[] { "https://images.unsplash.com/photo-1610433572201-110753c6cff9?w=800&h=600&fit=crop" },
                new[] { "CI/CD", "devops", "automation", "deployment" },
                80, 200, 400, 3, 5, 7
            ),

            // More Writing & Translation Gigs
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will write professional product descriptions",
                "professional-product-descriptions",
                "Compelling product descriptions that sell! SEO-optimized, conversion-focused copy for your e-commerce store. Multiple formats and styles available.",
                4.9m, 187, 9,
                new[] { "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=800&h=600&fit=crop" },
                new[] { "product descriptions", "copywriting", "ecommerce", "seo" },
                10, 25, 50, 1, 2, 3
            ),
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will proofread and edit your documents",
                "proofreading-editing-documents",
                "Polish your writing with professional proofreading and editing! Grammar, spelling, punctuation, and style corrections. Fast turnaround for any document type.",
                4.8m, 234, 12,
                new[] { "https://images.unsplash.com/photo-1503551723145-6c0407420518?w=800&h=600&fit=crop" },
                new[] { "proofreading", "editing", "grammar", "writing" },
                15, 40, 80, 1, 2, 3
            ),
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will write engaging social media posts",
                "engaging-social-media-posts",
                "Boost your social media engagement with catchy posts! Content for Instagram, Facebook, Twitter, LinkedIn. Hashtag research included. Multiple posts per order.",
                4.7m, 312, 15,
                new[] { "https://images.unsplash.com/photo-1611162617474-5b21e879e113?w=800&h=600&fit=crop" },
                new[] { "social media", "content writing", "instagram", "facebook" },
                20, 50, 100, 2, 3, 5
            ),

            // More Video & Animation Gigs
            CreateGig(
                users[3].Id, videoCategory.Id,
                "I will create professional product demo videos",
                "professional-product-demo-videos",
                "Showcase your product with engaging demo videos! High-quality production with music, graphics, and professional editing. Perfect for marketing and sales.",
                4.8m, 89, 4,
                new[] { "https://images.unsplash.com/photo-1536240478700-b869070f9279?w=800&h=600&fit=crop" },
                new[] { "product video", "demo video", "marketing video", "commercial" },
                60, 140, 280, 3, 5, 7
            ),
            CreateGig(
                users[3].Id, videoCategory.Id,
                "I will add subtitles and captions to your videos",
                "video-subtitles-captions",
                "Make your videos accessible with professional subtitles and captions! Multiple languages supported. SRT files included. Fast delivery and accurate transcription.",
                4.9m, 267, 11,
                new[] { "https://images.unsplash.com/photo-1611162616475-46b635cb6868?w=800&h=600&fit=crop" },
                new[] { "subtitles", "captions", "transcription", "accessibility" },
                15, 35, 70, 1, 2, 3
            ),
            CreateGig(
                users[12].Id, videoCategory.Id,
                "I will create logo animations and motion graphics",
                "logo-animations-motion-graphics",
                "Bring your logo to life with stunning animations! Professional motion graphics for intros, outros, and brand identity. After Effects animations included.",
                4.8m, 145, 6,
                new[] { "https://images.unsplash.com/photo-1626785774573-4b799314346d?w=800&h=600&fit=crop" },
                new[] { "logo animation", "motion graphics", "after effects", "intro" },
                50, 120, 250, 3, 5, 7
            ),

            // More Digital Marketing Gigs
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will create and manage your Google Ads campaigns",
                "google-ads-campaigns-management",
                "Drive traffic and conversions with optimized Google Ads! Campaign setup, keyword research, ad copy creation, and ongoing management. Detailed reporting included.",
                4.9m, 178, 5,
                new[] { "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=800&h=600&fit=crop" },
                new[] { "google ads", "ppc", "advertising", "campaign management" },
                100, 250, 500, 7, 14, 30
            ),
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will write SEO-optimized meta descriptions and titles",
                "seo-meta-descriptions-titles",
                "Improve your search rankings with optimized meta tags! SEO-friendly titles and descriptions for all your pages. Keyword research included.",
                4.8m, 423, 18,
                new[] { "https://images.unsplash.com/photo-1571786256017-aee7a0c009b6?w=800&h=600&fit=crop" },
                new[] { "SEO", "meta tags", "meta descriptions", "keyword research" },
                25, 60, 120, 1, 2, 3
            ),
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will create a comprehensive marketing strategy",
                "comprehensive-marketing-strategy",
                "Develop a winning marketing strategy for your business! Market analysis, competitor research, target audience identification, and action plan. Complete roadmap included.",
                4.9m, 94, 3,
                new[] { "https://images.unsplash.com/photo-1552664730-d307ca884978?w=800&h=600&fit=crop" },
                new[] { "marketing strategy", "business strategy", "consulting", "planning" },
                150, 350, 700, 7, 10, 14
            ),

            // More Music & Audio Gigs
            CreateGig(
                users[5].Id, musicCategory.Id,
                "I will create custom background music for your videos",
                "custom-background-music-videos",
                "Original background music tailored to your videos! Various genres and moods. Royalty-free and exclusive rights. Perfect for YouTube, commercials, and films.",
                4.8m, 156, 7,
                new[] { "https://images.unsplash.com/photo-1507838153414-b4b713384ebd?w=800&h=600&fit=crop" },
                new[] { "background music", "music production", "royalty free", "custom music" },
                40, 100, 200, 3, 5, 7
            ),
            CreateGig(
                users[5].Id, musicCategory.Id,
                "I will remove background noise from your audio",
                "remove-background-noise-audio",
                "Clean up your audio recordings! Professional noise removal and audio enhancement. Perfect for podcasts, interviews, and voice recordings.",
                4.9m, 289, 14,
                new[] { "https://images.unsplash.com/photo-1615655406736-b37c4fabf923?w=800&h=600&fit=crop" },
                new[] { "audio editing", "noise removal", "audio cleanup", "podcast" },
                20, 50, 100, 1, 2, 3
            ),
            CreateGig(
                users[11].Id, musicCategory.Id,
                "I will add sound effects to your video or audio",
                "sound-effects-video-audio",
                "Enhance your projects with professional sound effects! Extensive library of high-quality sounds. Perfect sync and timing. Multiple formats available.",
                4.8m, 112, 6,
                new[] { "https://images.unsplash.com/photo-1614149162883-504ce4d13909?w=800&h=600&fit=crop" },
                new[] { "sound effects", "audio design", "foley", "post production" },
                25, 60, 120, 2, 3, 5
            ),

            // More Business Gigs
            CreateGig(
                users[7].Id, businessCategory.Id,
                "I will create Excel spreadsheets and templates",
                "excel-spreadsheets-templates",
                "Professional Excel spreadsheets and templates for your business needs! Formulas, charts, automation, and dashboards. Customizable and user-friendly.",
                4.8m, 267, 13,
                new[] { "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800&h=600&fit=crop" },
                new[] { "excel", "spreadsheet", "templates", "data analysis" },
                20, 50, 100, 2, 3, 5
            ),
            CreateGig(
                users[13].Id, businessCategory.Id,
                "I will create PowerPoint presentations",
                "powerpoint-presentations-design",
                "Stunning PowerPoint presentations that impress! Professional design, animations, and templates. Perfect for business, education, or marketing.",
                4.9m, 334, 16,
                new[] { "https://images.unsplash.com/photo-1543269865-cbf427effbad?w=800&h=600&fit=crop" },
                new[] { "powerpoint", "presentation", "slides", "design" },
                30, 75, 150, 2, 4, 7
            ),
            CreateGig(
                users[7].Id, businessCategory.Id,
                "I will provide market research and analysis",
                "market-research-analysis",
                "Data-driven market research for your business! Industry analysis, competitor research, target audience insights, and strategic recommendations.",
                4.8m, 78, 2,
                new[] { "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&h=600&fit=crop" },
                new[] { "market research", "business analysis", "competitor research", "strategy" },
                100, 250, 500, 5, 7, 10
            ),

            // More AI Services Gigs
            CreateGig(
                users[6].Id, aiCategory.Id,
                "I will create AI-generated images and artwork",
                "ai-generated-images-artwork",
                "Stunning AI-generated images for your projects! Custom artwork, product photos, concept art, and marketing visuals. Multiple styles and variations.",
                4.9m, 156, 8,
                new[] { "https://images.unsplash.com/photo-1620641788421-7f1c338e420a?w=800&h=600&fit=crop" },
                new[] { "AI images", "ai art", "generated images", "midjourney" },
                25, 60, 120, 1, 2, 3
            ),
            CreateGig(
                users[6].Id, aiCategory.Id,
                "I will fine-tune AI models for your specific needs",
                "fine-tune-ai-models",
                "Custom AI models trained on your data! Fine-tuning GPT, BERT, or other models for specific tasks. Data preprocessing and model optimization included.",
                5.0m, 45, 1,
                new[] { "https://images.unsplash.com/photo-1620712943543-bcc4688e7485?w=800&h=600&fit=crop" },
                new[] { "AI training", "machine learning", "fine-tuning", "custom model" },
                300, 700, 1500, 10, 14, 21
            ),
            CreateGig(
                users[6].Id, aiCategory.Id,
                "I will create AI-powered data analysis scripts",
                "ai-powered-data-analysis",
                "Automate your data analysis with AI! Python scripts using machine learning for pattern recognition, predictions, and insights. Documentation included.",
                4.9m, 67, 3,
                new[] { "https://images.unsplash.com/photo-1518186285589-2f7649de83e0?w=800&h=600&fit=crop" },
                new[] { "AI analysis", "machine learning", "data science", "python" },
                100, 250, 500, 5, 7, 10
            ),
            CreateGig(
                users[6].Id, aiCategory.Id,
                "I will build an AI content generator",
                "ai-content-generator",
                "Automate content creation with AI! Custom content generator for blogs, social media, product descriptions, or emails. API integration available.",
                4.8m, 89, 4,
                new[] { "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=800&h=600&fit=crop" },
                new[] { "AI content", "content generator", "automation", "GPT" },
                150, 350, 700, 7, 10, 14
            )
        };

        await context.Gigs.AddRangeAsync(gigs);
        await context.SaveChangesAsync();

        // ============================================
        // ORDERS
        // ============================================
        var buyerIds = users.Skip(15).Take(10).Select(u => u.Id).ToList(); // Indices 15-24 are buyers
        var orders = new List<Order>();
        var statuses = new[] { "pending", "in_progress", "delivered", "completed", "completed", "completed" };

        // Create orders for each buyer
        foreach (var buyerId in buyerIds)
        {
            var randomGigs = gigs.OrderBy(_ => Random.Shared.Next()).Take(4).ToList();
            foreach (var gig in randomGigs)
            {
                var packageType = new[] { "basic", "standard", "premium" }[Random.Shared.Next(3)];
                var package = gig.Packages!.First(p => p.PackageType == packageType);
                var status = statuses[Random.Shared.Next(statuses.Length)];
                
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}-{Random.Shared.Next(1000)}",
                    GigId = gig.Id,
                    BuyerId = buyerId,
                    SellerId = gig.SellerId,
                    PackageType = packageType,
                    Price = package.Price,
                    ServiceFee = package.Price * 0.1m,
                    TotalPrice = package.Price * 1.1m,
                    Status = status,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60)),
                    DeliveryDeadline = DateTime.UtcNow.AddDays(package.DeliveryDays)
                };
                
                if (status == "completed")
                {
                    order.DeliveredAt = order.CreatedAt.AddDays(Random.Shared.Next(1, package.DeliveryDays));
                    order.CompletedAt = order.DeliveredAt?.AddDays(Random.Shared.Next(1, 3));
                }
                
                orders.Add(order);
            }
        }

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();

        // ============================================
        // REVIEWS (for completed orders)
        // ============================================
        var reviewComments = new[]
        {
            "Excellent work! Exactly what I was looking for. Highly recommended!",
            "Very professional and delivered on time. Will definitely order again.",
            "Amazing quality and great communication throughout the project.",
            "Exceeded my expectations! Thank you so much for the great work.",
            "Great attention to detail. Very satisfied with the result.",
            "Outstanding service! The quality was beyond what I expected.",
            "Quick turnaround and excellent quality. Highly recommend!",
            "Professional, creative, and easy to work with. Will be back!",
            "Fantastic experience from start to finish. 10/10 would recommend!",
            "Delivered exactly what I needed. Great communication throughout."
        };

        var sellerResponses = new[]
        {
            "Thank you so much! It was a pleasure working with you. Best of luck! 🎉",
            "I really appreciate the kind words! Looking forward to working with you again.",
            "Thank you! Don't hesitate to reach out if you need anything else.",
            "So glad you're happy with the result! Thank you for choosing me.",
            "Thanks for the amazing review! Wishing you success with your project!"
        };

        var reviews = new List<Review>();
        var completedOrders = orders.Where(o => o.Status == "completed").ToList();

        foreach (var order in completedOrders)
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                GigId = order.GigId,
                BuyerId = order.BuyerId,
                SellerId = order.SellerId,
                Rating = Random.Shared.Next(4, 6), // 4-5 stars
                Comment = reviewComments[Random.Shared.Next(reviewComments.Length)],
                SellerResponse = Random.Shared.Next(2) == 0 ? sellerResponses[Random.Shared.Next(sellerResponses.Length)] : null,
                CreatedAt = order.CompletedAt?.AddHours(Random.Shared.Next(1, 48)) ?? DateTime.UtcNow
            };
            reviews.Add(review);
        }

        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();

        // ============================================
        // TRANSACTIONS
        // ============================================
        var transactions = new List<Transaction>();
        
        foreach (var order in completedOrders)
        {
            // Buyer payment
            transactions.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                UserId = order.BuyerId,
                Amount = -order.TotalPrice,
                Type = "payment",
                Status = "completed",
                Description = $"Payment for order #{order.OrderNumber}",
                CreatedAt = order.CreatedAt
            });
            
            // Seller earning (minus platform fee)
            transactions.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                UserId = order.SellerId,
                Amount = order.Price * 0.8m, // 80% to seller
                Type = "earning",
                Status = "completed",
                Description = $"Earning from order #{order.OrderNumber}",
                CreatedAt = order.CompletedAt ?? order.CreatedAt
            });
        }

        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Database seeded successfully!");
        Console.WriteLine($"   📁 {categories.Count} categories");
        Console.WriteLine($"   👥 {users.Count} users ({users.Count(u => u.Role == "seller")} sellers, {users.Count(u => u.Role == "buyer")} buyers)");
        Console.WriteLine($"   🛍️  {gigs.Count} gigs/services");
        Console.WriteLine($"   📦 {orders.Count} orders");
        Console.WriteLine($"   ⭐ {reviews.Count} reviews");
        Console.WriteLine($"   💰 {transactions.Count} transactions");
        Console.WriteLine("");
        Console.WriteLine("🎉 All services are now available in the database!");
    }

    private static Gig CreateGig(
        Guid sellerId, Guid categoryId,
        string title, string slug, string description,
        decimal rating, int reviewCount, int ordersInQueue,
        string[] imageUrls, string[] tags,
        decimal basicPrice, decimal standardPrice, decimal premiumPrice,
        int basicDays, int standardDays, int premiumDays)
    {
        return new Gig
        {
            Id = Guid.NewGuid(),
            SellerId = sellerId,
            CategoryId = categoryId,
            Title = title,
            Slug = slug,
            Description = description,
            Rating = rating,
            ReviewCount = reviewCount,
            OrdersInQueue = ordersInQueue,
            IsActive = true,
            Images = imageUrls.Select((url, i) => new GigImage 
            { 
                ImageUrl = url, 
                IsPrimary = i == 0, 
                SortOrder = i 
            }).ToList(),
            Tags = tags.Select(t => new GigTag { Tag = t }).ToList(),
            Packages = new List<GigPackage>
            {
                new()
                {
                    PackageType = "basic",
                    Name = "Basic",
                    Price = basicPrice,
                    DeliveryDays = basicDays,
                    Revisions = "2",
                    Description = "Basic package"
                },
                new()
                {
                    PackageType = "standard",
                    Name = "Standard",
                    Price = standardPrice,
                    DeliveryDays = standardDays,
                    Revisions = "5",
                    Description = "Standard package with more features"
                },
                new()
                {
                    PackageType = "premium",
                    Name = "Premium",
                    Price = premiumPrice,
                    DeliveryDays = premiumDays,
                    Revisions = "Unlimited",
                    Description = "Premium package with all features"
                }
            }
        };
    }
}
