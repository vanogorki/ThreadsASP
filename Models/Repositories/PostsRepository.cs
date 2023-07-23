using Microsoft.EntityFrameworkCore;

namespace ThreadsASP.Models.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private AppDbContext context;

        public PostsRepository(AppDbContext ctx)
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

        public List<Post> GetUserPosts(string selectedUserId)
            => context.Posts.Where(p => p.AppUserId == selectedUserId).OrderByDescending(i => i.Id).ToList();
    }
}
