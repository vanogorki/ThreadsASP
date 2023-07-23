namespace ThreadsASP.FileUploadService
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment environment;
        public LocalFileService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public async Task UploadFileAsync(IFormFile file)
        {
            var filePath = Path.Combine(environment.ContentRootPath, @"wwwroot\images", file.FileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream); 
        }
        public static void DeleteImage(string? ImgName)
        {
            try
            {
                File.Delete(@$"wwwroot\images\{ImgName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
