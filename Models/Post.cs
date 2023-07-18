using System.ComponentModel.DataAnnotations;

namespace ThreadsASP.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Text { get; set; }
        [Required]
        public string? Date { get; set; }
    }
}
