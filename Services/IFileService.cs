namespace ThreadsASP.FileUploadService
{
    public interface IFileService
    {
        Task UploadPostImageAsync(IFormFile file, string newFileName);
        void UploadProfileImage(IFormFile file, string newFileName);
        static void DeleteImage(string? ImgName) { }
    }
}
