namespace ThreadsASP.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }
        public ApplicationUser? CurrentUser { get; set; }
    }
}
