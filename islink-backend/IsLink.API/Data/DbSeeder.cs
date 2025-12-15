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

        // Add languages to all users
        foreach (var user in users.Take(8))
        {
            user.Languages = new List<UserLanguage>
            {
                new() { LanguageName = "Arabic", Proficiency = "Native" },
                new() { LanguageName = "English", Proficiency = "Fluent" }
            };
        }
        
        // Add languages to buyers
        users[8].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" } };
        users[9].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" } };
        users[10].Languages = new List<UserLanguage> { new() { LanguageName = "English", Proficiency = "Native" }, new() { LanguageName = "Mandarin", Proficiency = "Fluent" } };
        users[11].Languages = new List<UserLanguage> { new() { LanguageName = "Spanish", Proficiency = "Native" }, new() { LanguageName = "English", Proficiency = "Fluent" } };
        users[12].Languages = new List<UserLanguage> { new() { LanguageName = "Korean", Proficiency = "Native" }, new() { LanguageName = "English", Proficiency = "Fluent" } };

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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/115528926/original/77cc7c95f5ebe4e46e84a5a59ac5d8e881f99b70/design-3-modern-minimalist-logo-design.png" },
                new[] { "logo design", "minimalist", "branding", "modern" },
                25, 50, 100, 3, 5, 7
            ),
            CreateGig(
                users[0].Id, graphicsCategory.Id,
                "I will create a complete brand identity package",
                "complete-brand-identity-package",
                "Get everything your brand needs to stand out! This package includes logo, business cards, letterhead, social media kit, and brand guidelines. Perfect for new businesses or rebrands looking for a cohesive visual identity.",
                4.9m, 45, 3,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/197515431/original/8f4e25c4cd0e1f4e9d8a5f6a7b8c9d0e1f2a3b4c/create-brand-identity-design.jpg" },
                new[] { "brand identity", "branding", "business cards", "logo" },
                150, 300, 500, 7, 10, 14
            ),
            CreateGig(
                users[0].Id, graphicsCategory.Id,
                "I will design stunning social media graphics",
                "stunning-social-media-graphics",
                "Boost your social media presence with eye-catching graphics! I create posts, stories, covers, and ads for Instagram, Facebook, LinkedIn, and more. Consistent branding that gets engagement.",
                4.8m, 38, 8,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/156789123/original/social-media-graphics.jpg" },
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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/197495498/original/43c71bce96e57eeb9efaa5c3e433c3a309b0a59a/develop-modern-responsive-website-in-react-js.jpg" },
                new[] { "react", "nodejs", "web development", "responsive" },
                100, 250, 500, 7, 14, 21
            ),
            CreateGig(
                users[1].Id, programmingCategory.Id,
                "I will develop a custom e-commerce website",
                "custom-ecommerce-website",
                "Launch your online store with a custom e-commerce solution! Built with modern technologies, including shopping cart, payment integration (Stripe/PayPal), inventory management, and admin dashboard. Fully responsive and SEO-optimized.",
                5.0m, 34, 2,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/ecommerce-website.jpg" },
                new[] { "ecommerce", "online store", "stripe", "shopify" },
                300, 600, 1000, 14, 21, 30
            ),
            CreateGig(
                users[1].Id, programmingCategory.Id,
                "I will fix bugs and optimize your website",
                "fix-bugs-optimize-website",
                "Having issues with your website? I'll fix bugs, improve performance, and optimize your code. Experienced with React, Angular, Vue, Node.js, PHP, and more. Fast turnaround with detailed documentation.",
                4.9m, 56, 6,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/bug-fix.jpg" },
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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/seo-writing.jpg" },
                new[] { "blog writing", "SEO", "content writing", "articles" },
                15, 30, 60, 2, 3, 5
            ),
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will translate documents from Arabic to English",
                "arabic-english-translation",
                "Professional Arabic-English translation services. Accurate, culturally-sensitive translations for business documents, marketing materials, legal papers, and more. Native Arabic speaker with 10+ years of translation experience.",
                4.9m, 41, 4,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/translation.jpg" },
                new[] { "translation", "arabic", "english", "documents" },
                20, 50, 100, 2, 4, 7
            ),
            CreateGig(
                users[2].Id, writingCategory.Id,
                "I will write compelling website copy",
                "compelling-website-copy",
                "Your website deserves words that convert! I write persuasive website copy that engages visitors and drives action. From landing pages to About Us sections, I'll craft copy that reflects your brand voice.",
                4.7m, 28, 5,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/copywriting.jpg" },
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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/youtube-editing.jpg" },
                new[] { "video editing", "youtube", "premiere pro", "motion graphics" },
                35, 75, 150, 3, 5, 7
            ),
            CreateGig(
                users[3].Id, videoCategory.Id,
                "I will create animated explainer videos",
                "animated-explainer-videos",
                "Explain your product or service with an engaging animated video! Perfect for startups, apps, and services. Includes script writing, voice-over coordination, and unlimited revisions.",
                4.8m, 22, 2,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/explainer-video.jpg" },
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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/social-media-management.jpg" },
                new[] { "social media", "marketing", "instagram", "facebook" },
                50, 150, 300, 7, 30, 30
            ),
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will run Facebook and Instagram ads campaigns",
                "facebook-instagram-ads-campaigns",
                "Get more leads and sales with targeted Facebook and Instagram ads! I'll create, manage, and optimize your ad campaigns for maximum ROI. Includes audience research, ad copy, and detailed reporting.",
                4.8m, 67, 4,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/facebook-ads.jpg" },
                new[] { "facebook ads", "instagram ads", "PPC", "advertising" },
                100, 250, 500, 7, 14, 30
            ),
            CreateGig(
                users[4].Id, marketingCategory.Id,
                "I will do complete SEO optimization for your website",
                "complete-seo-optimization",
                "Rank higher on Google with comprehensive SEO! Includes keyword research, on-page optimization, technical SEO audit, backlink strategy, and monthly progress reports. White-hat techniques only.",
                4.9m, 48, 3,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/seo-optimization.jpg" },
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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/mixing-mastering.jpg" },
                new[] { "mixing", "mastering", "music production", "audio" },
                50, 100, 200, 3, 5, 7
            ),
            CreateGig(
                users[5].Id, musicCategory.Id,
                "I will record a professional voice over",
                "professional-voice-over",
                "Need a professional voice for your project? I offer voice-over services in Arabic and English for commercials, explainer videos, audiobooks, and more. Fast delivery with unlimited revisions.",
                4.7m, 35, 7,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/voice-over.jpg" },
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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/ai-chatbot.jpg" },
                new[] { "AI chatbot", "GPT", "automation", "customer support" },
                200, 500, 1000, 7, 14, 21
            ),
            CreateGig(
                users[6].Id, aiCategory.Id,
                "I will automate your workflows with AI",
                "automate-workflows-ai",
                "Save time and reduce errors with AI automation! I'll analyze your workflows and implement AI solutions using Python, APIs, and machine learning. From data processing to report generation.",
                5.0m, 24, 1,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/ai-automation.jpg" },
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
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/business-plan.jpg" },
                new[] { "business plan", "startup", "financial projections", "strategy" },
                100, 250, 500, 5, 7, 14
            ),
            CreateGig(
                users[7].Id, businessCategory.Id,
                "I will be your virtual assistant for a week",
                "virtual-assistant-week",
                "Need help managing your workload? I provide professional virtual assistance including email management, scheduling, research, data entry, and customer support. Let me handle the busy work!",
                4.5m, 28, 4,
                new[] { "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/virtual-assistant.jpg" },
                new[] { "virtual assistant", "admin support", "data entry", "scheduling" },
                50, 100, 200, 7, 7, 7
            )
        };

        await context.Gigs.AddRangeAsync(gigs);
        await context.SaveChangesAsync();

        // ============================================
        // ORDERS
        // ============================================
        var buyerIds = users.Skip(8).Take(5).Select(u => u.Id).ToList(); // Last 5 are buyers
        var orders = new List<Order>();
        var statuses = new[] { "pending", "in_progress", "delivered", "completed", "completed", "completed" };

        // Create orders for each buyer
        foreach (var buyerId in buyerIds)
        {
            var randomGigs = gigs.OrderBy(_ => Random.Shared.Next()).Take(3).ToList();
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
        Console.WriteLine($"   - {categories.Count} categories");
        Console.WriteLine($"   - {users.Count} users ({users.Count(u => u.Role == "seller")} sellers, {users.Count(u => u.Role == "buyer")} buyers)");
        Console.WriteLine($"   - {gigs.Count} gigs");
        Console.WriteLine($"   - {orders.Count} orders");
        Console.WriteLine($"   - {reviews.Count} reviews");
        Console.WriteLine($"   - {transactions.Count} transactions");
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
