using Microsoft.AspNetCore.Identity;

namespace ThreadsASP.Models.ViewModels
{
    public class UserPostViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
            = Enumerable.Empty<Post>();

        public ApplicationUser? SelectedUser { get; set; }

        public bool? IsCurrentUser { get; set; }
        
        public ApplicationUser? CurrentUser { get; set; }
    }
}
