namespace ThreadsASP.Models
{
    public interface IFollowsRepository
    {
        IQueryable<Follow> Follows { get; }
        void Follow(ApplicationUser currentUser, ApplicationUser selectedUser);
        void UnFollow(Follow f);
        bool IsFollowing(string currentUserId, string selectedUserId);
    }
}
