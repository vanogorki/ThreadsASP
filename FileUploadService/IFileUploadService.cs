namespace ThreadsASP.FileUploadService
{
    public interface IFileUploadService
    {
        Task UploadFileAsync(IFormFile file);
    }
}
