namespace ThreadsASP.FileUploadService
{
    public interface IFileService
    {
        Task UploadFileAsync(IFormFile file);
        static void DeleteImage(string? ImgName) { }
    }
}
