using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Gymbokning.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GymClass> GymClasses { get; set; }
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for ApplicationUserGymClass
            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasKey(aug => new { aug.ApplicationUserId, aug.GymClassId });

            // Configure relationships
            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(aug => aug.ApplicationUser)
                .WithMany(u => u.AttendedClasses)
                .HasForeignKey(aug => aug.ApplicationUserId);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(aug => aug.GymClass)
                .WithMany(gc => gc.AttendingMembers)
                .HasForeignKey(aug => aug.GymClassId);
        }
    }
}
