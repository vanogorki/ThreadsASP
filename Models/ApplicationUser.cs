using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ThreadsASP.Models
{
    public class ApplicationUser : IdentityUser
    {
        private AppDbContext context;
        
        public ApplicationUser(AppDbContext ctx)
        {
            context = ctx;
        }
        public ApplicationUser() { }
        
        public string ProfileImgName { get; set; } = "Default.png";
        
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
