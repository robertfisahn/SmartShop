using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models;

namespace SmartShopAPI.Repositories
{
    public class CategoryRepository(SmartShopDbContext context) : ICategoryRepository
    {
        public List<Category> GetAll() => context.Categories.ToList();
        public Category? GetCategory(int categoryId) => context.Categories.FirstOrDefault(x => x.Id == categoryId);
        public void Create(Category category) => context.Categories.Add(category);
        public void Delete(Category category) => context.Categories.Remove(category);
        public void Update(Category category) => context.Categories.Update(category);
        public async Task<bool> ExistsAsync(int categoryId) => await context.Categories.AnyAsync(x => x.Id == categoryId);
        public void SaveChanges() => context.SaveChanges();
    }
}
