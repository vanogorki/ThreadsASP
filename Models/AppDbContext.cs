using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

            builder.Entity<Follow>()
                .HasKey(x => new { x.FollowingUserId, x.FollowerUserId });

            builder.Entity<Follow>()
                .HasOne(x => x.FollowingUser)
                .WithMany(z => z.SendFollows)
                .HasForeignKey(k => k.FollowingUserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<Follow>()
                .HasOne(x => x.FollowerUser)
                .WithMany(z => z.ReceiveFollows)
                .HasForeignKey(k => k.FollowerUserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<Like>()
                .HasKey(x => new { x.UserId, x.PostId });

            builder.Entity<Like>()
                .HasOne(u => u.User)
                .WithMany(l => l.SendLikes)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<Like>()
                .HasOne(p => p.Post)
                .WithMany(l => l.Likes)
                .HasForeignKey(k => k.PostId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}