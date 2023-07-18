using ThreadsASP.Models;

namespace SportsStore.Models
{
    public interface IPostsRepository
    {
        IQueryable<Post> Posts { get; }

        void CreatePost(Post p);
        void DeletePost(Post p);
    }
}