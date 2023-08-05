using Microsoft.EntityFrameworkCore;

namespace ThreadsASP.Models.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private AppDbContext _context;

        public ReportsRepository(AppDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<Report> Reports => _context.Reports.Include(x => x.ReportSender).Include(x => x.ReportedPost).Include(x => x.ReportedUser);

        public void AddReport(Report report)
        {
            _context.Add(report);
            _context.SaveChanges();
        }

        public void DeleteReport(Report report)
        {
            _context.Remove(report);
            _context.SaveChanges();
        }
    }
}
