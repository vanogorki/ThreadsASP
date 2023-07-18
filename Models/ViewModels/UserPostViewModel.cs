using Microsoft.AspNetCore.Identity;

namespace ThreadsASP.Models.ViewModels
{
    public class UserPostViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
            = Enumerable.Empty<Post>();

        public string? UserName { get; set; }
    }
}
