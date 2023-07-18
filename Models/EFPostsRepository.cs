using SportsStore.Models;

namespace ThreadsASP.Models
{
    public class EFPostsRepository : IPostsRepository
    {
        private ThreadsDbContext context;

        public EFPostsRepository(ThreadsDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Post> Posts => context.Posts;

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
