namespace ThreadsASP.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private AppDbContext _context;

        public UserRepository(AppDbContext ctx)
        {
            _context = ctx;
        }

        public void SaveNewProfilePicture()
        {
            _context.SaveChanges();
        }
    }
}
