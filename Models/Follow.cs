using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreadsASP.Models
{
    public class Follow
    {
        public string FollowingUserId { get; set; }
        [ForeignKey("FollowingUserId")]
        public ApplicationUser FollowingUser { get; set; }
        public string FollowerUserId { get; set; }
        [ForeignKey("FollowerUserId")]
        public ApplicationUser FollowerUser { get; set; }
    }
}
