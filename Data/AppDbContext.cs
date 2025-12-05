using Microsoft.EntityFrameworkCore;
using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // تعريف الجداول (ضروري عشان Repository يقدر يوصل للبيانات)
        public DbSet<User> Users { get; set; }
        public DbSet<Gig> Gigs { get; set; }
        public DbSet<GigImage> GigImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        // زميلك بكمل الباقي (Orders, Messages, etc)
    }
}