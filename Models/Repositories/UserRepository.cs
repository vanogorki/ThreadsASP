namespace ThreadsASP.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private AppDbContext context;

        public UserRepository(AppDbContext ctx)
        {
            context = ctx;
        }

        public void SaveNewProfilePicture()
        {
            context.SaveChanges();
        }
    }
}
