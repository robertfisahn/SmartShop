using AutoMapper;

using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Services.Core
{
    public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
    {
        public async Task<List<CategoryDto>> GetAll()
        {
            var categories = await categoryRepository.GetAllAsync();
            return mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetById(int categoryId)
        {
            var category = await categoryRepository.GetAsync(categoryId) ?? throw new NotFoundException("Category not found");
            return mapper.Map<CategoryDto>(category);
        }
        public async Task<int> Create(CategoryUpsertDto dto)
        {
            var category = mapper.Map<Category>(dto);
            await categoryRepository.AddAsync(category);
            await categoryRepository.SaveChangesAsync();
            return category.Id;
        }
        public async Task Delete(int categoryId)
        {
            var category = await categoryRepository.GetAsync(categoryId) ?? throw new NotFoundException("Category not found");
            categoryRepository.Delete(category);
            await categoryRepository.SaveChangesAsync();
        }
        public async Task Update(int categoryId, CategoryUpsertDto dto)
        {
            var category = await categoryRepository.GetAsync(categoryId) ?? throw new NotFoundException("Category not found");
            mapper.Map(dto, category);
            await categoryRepository.SaveChangesAsync();
        }
    }
}
