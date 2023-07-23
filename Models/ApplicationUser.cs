using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ThreadsASP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfileImgName { get; set; } = "Default.jpg";
        public ICollection<Post> Posts { get; set; }
        public ICollection<Follow> SendFollows { get; set; }
        public ICollection<Follow> ReceiveFollows { get; set; }
    }
}
