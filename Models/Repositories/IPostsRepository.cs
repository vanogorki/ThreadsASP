﻿namespace ThreadsASP.Models.Repositories
{
    public interface IPostsRepository
    {
        IQueryable<Post> Posts { get; }

        void CreatePost(Post p);
        void DeletePost(Post p);
    }
}