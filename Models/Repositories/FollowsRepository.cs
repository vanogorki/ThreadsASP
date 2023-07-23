using Microsoft.EntityFrameworkCore;

namespace ThreadsASP.Models
{
    public class FollowsRepository : IFollowsRepository
    {
        private AppDbContext context;

        public FollowsRepository(AppDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Follow> Follows => context.Follows.Include(x => x.FollowingUser).Include(x => x.FollowerUser);

        public void Follow(ApplicationUser currentUser, ApplicationUser selectedUser)
        {
            var newFollow = new Follow
            {
                FollowingUser = currentUser,
                FollowingUserId = currentUser.Id,
                FollowerUser = selectedUser,
                FollowerUserId = selectedUser.Id
            };
            context.Add(newFollow);
            context.SaveChanges();
        }

        public void UnFollow(Follow f)
        {
            context.Remove(f);
            context.SaveChanges();
        }

        public bool IsFollowing(string currentUserId, string selectedUserId)
            => (context.Follows.FirstOrDefault(x => x.FollowingUserId == currentUserId && x.FollowerUserId == selectedUserId) != null) ? true : false;
    }
}
