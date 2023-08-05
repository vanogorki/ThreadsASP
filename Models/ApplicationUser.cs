using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ThreadsASP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
        public string ProfileImgName { get; set; } = "Default.jpg";
        public bool IsBlocked { get; set; } = false;
        public ICollection<Post> Posts { get; set; }
        public ICollection<Follow> SendFollows { get; set; }
        public ICollection<Follow> ReceiveFollows { get; set; }
        public ICollection<Like> SendLikes { get; set; }
    }
}
