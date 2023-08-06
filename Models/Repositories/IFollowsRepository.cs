namespace ThreadsASP.Models
{
    public interface IFollowsRepository
    {
        IQueryable<Follow> Follows { get; }
        void Follow(Follow f);
        void UnFollow(Follow f);
        bool IsFollowing(string currentUserId, string selectedUserId);
    }
}
