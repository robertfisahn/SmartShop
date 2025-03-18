using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly SmartShopDbContext _context;
        private readonly IMapper _mapper;
        public CategoryService(SmartShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IEnumerable<CategoryDto> GetAll()
        {
            var categories = _context.Categories
                .ToList();
            var dto = _mapper.Map<List<CategoryDto>>(categories);
            return dto;
        }

        public CategoryDto GetCategory(int categoryId)
        {
            var category = _context.Categories
                .Include(x => x.Products)
                .FirstOrDefault(c => c.Id == categoryId) ?? throw new NotFoundException("Category not found");
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }
        public int Create(CategoryUpsertDto dto)
        {
            var entityCategory = _mapper.Map<Category>(dto);
            _context.Categories.Add(entityCategory);
            _context.SaveChanges();
            return entityCategory.Id;
        }
        public void Delete(int categoryId)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.Id == categoryId) ?? throw new NotFoundException("Category not found");
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
        public void Update(int categoryId, CategoryUpsertDto dto)
        {
            var category = _context.Categories
                .FirstOrDefault(x => x.Id == categoryId) ?? throw new NotFoundException("Category not found");
            _mapper.Map(dto, category);
            _context.SaveChanges();
        }
    }
}
