using Microsoft.EntityFrameworkCore;
using IsLink.API.Models.Entities;

namespace IsLink.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<UserLanguage> UserLanguages => Set<UserLanguage>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Gig> Gigs => Set<Gig>();
    public DbSet<GigPackage> GigPackages => Set<GigPackage>();
    public DbSet<PackageFeature> PackageFeatures => Set<PackageFeature>();
    public DbSet<GigImage> GigImages => Set<GigImage>();
    public DbSet<GigTag> GigTags => Set<GigTag>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDelivery> OrderDeliveries => Set<OrderDelivery>();
    public DbSet<DeliveryAttachment> DeliveryAttachments => Set<DeliveryAttachment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================
        // User Configuration
        // ============================================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Rating).HasPrecision(2, 1);
            entity.Property(e => e.Balance).HasPrecision(12, 2);
        });

        // ============================================
        // UserSkill - Composite Key
        // ============================================
        modelBuilder.Entity<UserSkill>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SkillName });
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Skills)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // UserLanguage - Composite Key
        // ============================================
        modelBuilder.Entity<UserLanguage>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LanguageName });
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Languages)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // Category - Self-referencing
        // ============================================
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            
            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.Subcategories)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // Gig Configuration
        // ============================================
        modelBuilder.Entity<Gig>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.SellerId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.IsActive);
            
            entity.Property(e => e.Rating).HasPrecision(2, 1);

            entity.HasOne(e => e.Seller)
                  .WithMany(u => u.Gigs)
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Gigs)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // GigPackage Configuration
        // ============================================
        modelBuilder.Entity<GigPackage>(entity =>
        {
            entity.HasIndex(e => new { e.GigId, e.PackageType }).IsUnique();
            entity.Property(e => e.Price).HasPrecision(12, 2);

            entity.HasOne(e => e.Gig)
                  .WithMany(g => g.Packages)
                  .HasForeignKey(e => e.GigId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // GigTag - Composite Key
        // ============================================
        modelBuilder.Entity<GigTag>(entity =>
        {
            entity.HasKey(e => new { e.GigId, e.Tag });
            
            entity.HasOne(e => e.Gig)
                  .WithMany(g => g.Tags)
                  .HasForeignKey(e => e.GigId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // Order Configuration
        // ============================================
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.BuyerId);
            entity.HasIndex(e => e.SellerId);
            entity.HasIndex(e => e.Status);

            entity.Property(e => e.Price).HasPrecision(12, 2);
            entity.Property(e => e.ServiceFee).HasPrecision(12, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(12, 2);

            entity.HasOne(e => e.Buyer)
                  .WithMany(u => u.BuyerOrders)
                  .HasForeignKey(e => e.BuyerId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Seller)
                  .WithMany(u => u.SellerOrders)
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // Review Configuration
        // ============================================
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasIndex(e => e.OrderId).IsUnique();
            entity.HasIndex(e => e.GigId);

            entity.HasOne(e => e.Order)
                  .WithOne(o => o.Review)
                  .HasForeignKey<Review>(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Buyer)
                  .WithMany(u => u.BuyerReviews)
                  .HasForeignKey(e => e.BuyerId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Seller)
                  .WithMany(u => u.SellerReviews)
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ============================================
        // Favorite - Composite Key
        // ============================================
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GigId });

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Favorites)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Gig)
                  .WithMany(g => g.Favorites)
                  .HasForeignKey(e => e.GigId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // Transaction Configuration
        // ============================================
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(12, 2);
        });
    }
}

