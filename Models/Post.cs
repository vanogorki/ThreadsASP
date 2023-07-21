using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreadsASP.Models
{
    public class Post
    {
        [Key]
        public long PostId { get; set; }
        public string PostUserName { get; set; } = string.Empty;
        [Required]
        [MaxLength(300)]
        public string Text { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string? ImgName { get; set; }
    }
}
