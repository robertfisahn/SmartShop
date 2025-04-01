namespace SmartShopAPI.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImage(string imagePath);
    }
}
