using Microsoft.EntityFrameworkCore;

namespace ThreadsASP.Models.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private AppDbContext _context;

        public PostsRepository(AppDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<Post> Posts => _context.Posts.Include(x => x.AppUser);

        public void DeletePost(Post p)
        {
            _context.Remove(p);
            _context.SaveChanges();
        }

        public void CreatePost(Post p)
        {
            _context.Add(p);
            _context.SaveChanges();
        }

        public List<Post> GetUserPosts(string selectedUserId)
            => _context.Posts.Where(p => p.AppUserId == selectedUserId).OrderByDescending(i => i.Id).ToList();
    }
}
