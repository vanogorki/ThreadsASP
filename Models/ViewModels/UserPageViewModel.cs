﻿namespace ThreadsASP.Models.ViewModels
{
    public class UserPageViewModel
    {
        public List<Post> Posts { get; set; }
        public bool IsFollowing { get; set; }
        public ApplicationUser SelectedUser { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public bool? IsCurrentUser { get; set; }
        public int? FollowingCount { get; set; }
        public int? FollowersCount { get; set; }
    }
}
