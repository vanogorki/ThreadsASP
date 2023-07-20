namespace ThreadsASP.FileUploadService
{
    public class LocalFileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment environment;
        public LocalFileUploadService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public async Task UploadFileAsync(IFormFile file)
        {
            var filePath = Path.Combine(environment.ContentRootPath, @"wwwroot\images", file.FileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream); 
        }
    }
}
