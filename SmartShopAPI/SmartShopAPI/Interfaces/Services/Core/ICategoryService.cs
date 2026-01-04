using Microsoft.AspNetCore.Mvc;

using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface ICategoryService
    {
        Task<int> Create(CategoryUpsertDto dto);
        Task<List<CategoryDto>> GetAll();
        Task<CategoryDto> GetById(int categoryId);
        Task Update(int categoryId, CategoryUpsertDto dto);
        Task Delete(int categoryId);
    }
}
