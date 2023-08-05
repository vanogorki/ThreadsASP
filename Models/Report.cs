namespace ThreadsASP.Models
{
    public class Report
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public ApplicationUser ReportSender { get; set; }
        public Post? ReportedPost { get; set; }
        public ApplicationUser ReportedUser { get; set; }
    }
}
