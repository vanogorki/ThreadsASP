namespace ThreadsASP.Models
{
    public class Like
    {
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public Post Post { get; set; }
        public long PostId { get; set; }
    }
}
