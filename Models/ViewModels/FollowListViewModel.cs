namespace ThreadsASP.Models.ViewModels
{
    public class FollowListViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public ApplicationUser SelectedUser { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public List<Follow> CurrentUserFollows { get; set; }
    }
}
