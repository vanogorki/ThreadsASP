namespace ThreadsASP.Models.Repositories
{
    public interface ILikesRepository
    {
        IQueryable<Like> Likes { get; }
        void AddLike(Like like);
        void RemoveLike(Like like);
    }
}
