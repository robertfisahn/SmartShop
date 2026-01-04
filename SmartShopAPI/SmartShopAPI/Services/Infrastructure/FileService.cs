using SmartShopAPI.Interfaces.Services.Infrastructure;

namespace SmartShopAPI.Services.Infrastructure
{
    public class FileService : IFileService
    {
        private readonly string _uploadsFolder;

        public FileService()
        {
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(_uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine("images", "products", uniqueFileName);
        }

        public void DeleteImage(string imagePath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
