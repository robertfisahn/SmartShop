using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category? GetCategory(int categoryId);
        void Create(Category category);
        void Delete(Category category);
        void Update(Category category);
        Task<bool> ExistsAsync(int categoryId);
        void SaveChanges();
    }
}
