using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ThreadsASP.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .HasOne(c => c.AppUser)
                .WithMany(p => p.Posts)
                .HasForeignKey(p => p.AppUserId)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .HasMany(x => x.Posts)
                .WithOne(z => z.AppUser)
                .HasForeignKey(z => z.AppUserId)
                .IsRequired();

            builder.Entity<Follow>()
                .HasKey(x => new { x.FollowingUserId, x.FollowerUserId });

            builder.Entity<Follow>()
                .HasOne(x => x.FollowingUser)
                .WithMany(z => z.SendFollows)
                .HasForeignKey(k => k.FollowingUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Follow>()
                .HasOne(x => x.FollowerUser)
                .WithMany(z => z.ReceiveFollows)
                .HasForeignKey(k => k.FollowerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Follow> Follows { get; set; }
    }
}