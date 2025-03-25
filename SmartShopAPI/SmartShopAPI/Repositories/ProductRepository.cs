using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models;

namespace SmartShopAPI.Repositories
{
    public class ProductRepository(SmartShopDbContext context) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllAsync() => await context.Products.ToListAsync();
        public async Task<Product?> GetByIdAsync(int id) => await context.Products.FindAsync(id);
        public async Task<List<Product>> GetBySearchPhraseAsync(string searchPhrase) => await context.Products
            .Where(p => p.Name.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase)).ToListAsync();
        public async Task AddAsync(Product product) => await context.Products.AddAsync(product);
        public void Delete(Product product) => context.Products.Remove(product);
        public async Task<bool> ExistsByNameAsync(string productName, int? productId) => await context.Products
                .AnyAsync(p => p.Name == productName && (productId == null || p.Id != productId));
        public async Task UpdateStockQuantity(IEnumerable<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                var product = await context.Products.SingleOrDefaultAsync(x => x.Id == item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                }
            }
        }

        public async Task<List<Product>> GetByCategoryAndSearchPhraseAsync(int categoryId, string? searchPhrase)
        {
            return await context.Products
                .Where(x => x.CategoryId == categoryId &&
                            (searchPhrase == null || x.Name.ToLower().Contains(searchPhrase.ToLower())))
                .ToListAsync();
        }

        public async Task SaveChangesAsync() => await context.SaveChangesAsync();
    }
}
