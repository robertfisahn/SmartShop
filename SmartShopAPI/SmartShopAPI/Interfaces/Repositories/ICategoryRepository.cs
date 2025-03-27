using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetAsync(int categoryId);
        Task AddAsync(Category category);
        void Delete(Category category);
        Task<bool> ExistsAsync(int categoryId);
        Task SaveChangesAsync();
    }
}
