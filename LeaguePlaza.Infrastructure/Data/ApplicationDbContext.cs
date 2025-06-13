using LeaguePlaza.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeaguePlaza.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<QuestEntity> Quests { get; set; }
        public DbSet<MountEntity> Mounts { get; set; }
        public DbSet<MountRentalEntity> MountRentals { get; set; }
        public DbSet<MountRatingEntity> MountRatings { get; set; }
        public DbSet<ProductEntity> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasMany(au => au.PostedQuests)
                .WithOne(q => q.Creator)
                .HasForeignKey(q => q.CreatorId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<ApplicationUser>()
                .HasMany(au => au.AcceptedQuests)
                .WithOne(q => q.Adventurer)
                .HasForeignKey(q => q.AdventurerId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(builder);
        }
    }
}
