using Microsoft.EntityFrameworkCore;

namespace ThreadsASP.Models
{
    public class ThreadsDbContext : DbContext
    {
        public ThreadsDbContext(DbContextOptions<ThreadsDbContext> opts)
            : base(opts) { }
        public DbSet<Post> Posts => Set<Post>();
    }
}
