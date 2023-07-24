using Microsoft.AspNetCore.Identity;

namespace ThreadsASP.Models.ViewModels
{
    public class UserPageViewModel
    {
        public List<Post> Posts { get; set; }
        public bool IsFollowing { get; set; }

        public ApplicationUser? SelectedUser { get; set; }

        public bool? IsCurrentUser { get; set; }
    }
}
