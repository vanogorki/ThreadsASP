using Microsoft.AspNetCore.Identity;

namespace ThreadsASP.Models.ViewModels
{
    public class UserPostViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
            = Enumerable.Empty<Post>();

        public IdentityUser? SelectedUser { get; set; }

        public bool? IsCurrentUser { get; set; }
        
        public IdentityUser? CurrentUser { get; set; }
    }
}
