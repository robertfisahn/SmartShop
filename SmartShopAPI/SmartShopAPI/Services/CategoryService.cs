using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Services
{
    public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
    {
        public List<CategoryDto> GetAll()
        {
            var categories = categoryRepository.GetAll();
            return mapper.Map<List<CategoryDto>>(categories);
        }

        public CategoryDto GetCategory(int categoryId)
        {
            var category = categoryRepository.GetCategory(categoryId) ?? throw new NotFoundException("Category not found");
            return mapper.Map<CategoryDto>(category);
        }
        public int Create(CategoryUpsertDto dto)
        {
            var category = mapper.Map<Category>(dto);
            categoryRepository.Create(category);
            categoryRepository.SaveChanges();
            return category.Id;
        }
        public void Delete(int categoryId)
        {
            var category = categoryRepository.GetCategory(categoryId) ?? throw new NotFoundException("Category not found");
            categoryRepository.Delete(category);
            categoryRepository.SaveChanges();
        }
        public void Update(int categoryId, CategoryUpsertDto dto)
        {
            var category = categoryRepository.GetCategory(categoryId) ?? throw new NotFoundException("Category not found");
            mapper.Map(dto, category);
            categoryRepository.Update(category);
            categoryRepository.SaveChanges();
        }
    }
}
