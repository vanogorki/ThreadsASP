using Microsoft.EntityFrameworkCore;

namespace ThreadsASP.Models.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private AppDbContext _context;

        public LikesRepository(AppDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<Like> Likes => _context.Likes.Include(x => x.User).Include(z => z.Post);

        public void AddLike(Like like)
        {
            _context.Likes.Add(like);
            _context.SaveChanges();
        }

        public void RemoveLike(Like like)
        {
            _context.Likes.Remove(like);
            _context.SaveChanges();
        }
    }
}
