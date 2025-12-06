using Microsoft.EntityFrameworkCore;
using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

       // Defining tables (necessary so that Repository can access the data)
        public DbSet<User> Users { get; set; }
        public DbSet<Gig> Gigs { get; set; }
        public DbSet<GigImage> GigImages { get; set; }
        public DbSet<Tag> Tags { get; set; }

      // Supplementing tables from existing entities
        public DbSet<Category> Categories { get; set; }
        public DbSet<GigPackage> GigPackages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
