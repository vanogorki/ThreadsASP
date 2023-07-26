

namespace ThreadsASP.FileUploadService
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        public LocalFileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task UploadPostImageAsync(IFormFile file, string newFileName)
        {
            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\images", newFileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);
        }

        public void UploadProfileImage(IFormFile file, string newFileName)
        {
            try
            {
                using (var image = Image.Load(file.OpenReadStream()))
                {
                    var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\images", newFileName);
                    image.Mutate(x => x.Resize(300, 300));
                    image.Save(filePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
