using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

namespace ThreadsASP.Models
{
    public class EFPostsRepository : IPostsRepository
    {
        private AppDbContext context;

        public EFPostsRepository(AppDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Post> Posts => context.Posts.Include(x => x.AppUser);

        public void DeletePost(Post p)
        {
            context.Remove(p);
            context.SaveChanges();
        }

        public void CreatePost(Post p)
        {
            context.Add(p);
            context.SaveChanges();
        }
    }
}
