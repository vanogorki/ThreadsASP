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

        public IQueryable<Post> Posts => _context.Posts.Include(x => x.AppUser).Include(z => z.Repost.AppUser).Include(c => c.Likes);

        public void DeletePost(Post p)
        {
            if (p.Likes.Count() != 0)
            {
                foreach (var likes in p.Likes)
                {
                    _context.Remove(likes);
                }
            }
            if (Posts.Where(x => x.RepostId == p.Id) != null)
            {
                foreach (var repost in Posts.Where(x => x.RepostId == p.Id))
                {
                    _context.Remove(repost);
                }
            }
            _context.Remove(p);
            _context.SaveChanges();
        }

        public void CreatePost(Post p)
        {
            _context.Add(p);
            _context.SaveChanges();
        }
    }
}
