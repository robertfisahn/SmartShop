using Microsoft.AspNetCore.Mvc;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Interfaces.Services
{
    public interface ICategoryService
    {
        int Create(CategoryUpsertDto dto);
        void Delete(int categoryId);
        IEnumerable<CategoryDto> GetAll();
        CategoryDto GetCategory(int categoryId);
        void Update(int categoryId, CategoryUpsertDto dto);
    }
}