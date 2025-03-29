using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.Product;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IProductService
    {
        Task<int> Create(UpsertProductDto dto, IFormFile? file);
        Task Delete(int productId);
        Task<List<ProductDto>> GetAll();
        Task<List<ProductDto>> Search(string searchPhrase);
        Task<PagedResult<ProductDto>> GetByCategory(int categoryId, QueryParams query);
        Task<ProductDto> GetById(int productId);
        Task Update(int productId, UpsertProductDto dto, IFormFile? file);
        Task UpdateStock(List<OrderItem> orderItems);
        Task EnsureUniqueName(string productName, int? productId);
    }
}