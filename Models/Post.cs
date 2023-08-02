using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace ThreadsASP.Models
{
    public class Post
    {
        [Key]
        public long Id { get; set; }
        public ApplicationUser AppUser { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        [Required]
        [MaxLength(300)]
        public string Text { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string? ImgName { get; set; }
        public bool IsReposted => RepostId != null;
        public Post? Repost { get; set; }
        public long? RepostId { get; set; }
        public int RepostsCount { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
