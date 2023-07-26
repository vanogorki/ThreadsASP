using Microsoft.EntityFrameworkCore;

namespace ThreadsASP.Models
{
    public class FollowsRepository : IFollowsRepository
    {
        private AppDbContext _context;

        public FollowsRepository(AppDbContext ctx)
        {
            _context = ctx;
        }
        public IQueryable<Follow> Follows => _context.Follows.Include(x => x.FollowingUser).Include(x => x.FollowerUser);

        public void Follow(ApplicationUser currentUser, ApplicationUser selectedUser)
        {
            var newFollow = new Follow
            {
                FollowingUser = currentUser,
                FollowingUserId = currentUser.Id,
                FollowerUser = selectedUser,
                FollowerUserId = selectedUser.Id
            };
            _context.Add(newFollow);
            _context.SaveChanges();
        }

        public void UnFollow(Follow f)
        {
            _context.Remove(f);
            _context.SaveChanges();
        }

        public bool IsFollowing(string currentUserId, string selectedUserId)
            => (_context.Follows.FirstOrDefault(x => x.FollowingUserId == currentUserId && x.FollowerUserId == selectedUserId) != null) ? true : false;
    }
}
