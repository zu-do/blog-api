namespace TheBlog_API.Services.Interfaces
{
    public interface IImageService
    {
        void DeleteImage(string imageName);
        Task<string> SaveImage(IFormFile imageFile);
    }
}