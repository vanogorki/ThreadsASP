namespace ThreadsASP.Models.Repositories
{
    public interface IReportsRepository
    {
        IQueryable<Report> Reports { get; }

        void AddReport(Report report);
        void DeleteReport(Report report);
    }
}
