namespace SmartShopAPI.Interfaces.Services.Infrastructure
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImage(string imagePath);
    }
}
