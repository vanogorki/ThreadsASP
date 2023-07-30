namespace ThreadsASP.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public List<ApplicationUser>? SearchUsers { get; set; }
        public List<Post>? SearchPosts { get; set; }
        public string SearchString { get; set; }
        public string CurrentUserId { get; set; }
        public List<Follow> CurrentUserFollows { get; set; }
    }
}
