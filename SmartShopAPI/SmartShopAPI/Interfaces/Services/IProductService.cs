using SmartShopAPI.Entities;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.Product;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IProductService
    {
        Task<int> CreateAsync(UpsertProductDto dto, IFormFile? file);
        Task DeleteAsync(int productId);
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<List<ProductDto>> GetProductsAsync(string searchPhrase);
        Task<PagedResult<ProductDto>> GetAsync(int categoryId, QueryParams query);
        Task<ProductDto> GetByIdAsync(int productId);
        Task UpdateAsync(int productId, UpsertProductDto dto, IFormFile? file);
        Task CheckUniqueNameAsync(string productName, int? productId);
        void UpdateStockQuantity(IEnumerable<OrderItem> orderItems);
    }
}