using SmartShopAPI.Entities;
using SmartShopAPI.Models;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetBySearchPhraseAsync(string searchPhrase);
        Task AddAsync(Product product);
        void Delete(Product product);
        Task<bool> ExistsByNameAsync(string productName, int? productId);
        Task UpdateStockQuantity(IEnumerable<OrderItem> orderItems);
        Task<List<Product>> GetByCategoryAndSearchPhraseAsync(int categoryId, string? searchPhrase);
        Task SaveChangesAsync();
    }
}
