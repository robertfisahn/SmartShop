using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Data;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models;

namespace SmartShopAPI.Repositories
{
    public class CategoryRepository(SmartShopDbContext context) : ICategoryRepository
    {
        public async Task<List<Category>> GetAllAsync() => await context.Categories.ToListAsync();
        public async Task<Category?> GetAsync(int categoryId) => await context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
        public async Task AddAsync(Category category) => await context.Categories.AddAsync(category);
        public void Delete(Category category) => context.Categories.Remove(category);
        public async Task<bool> ExistsAsync(int categoryId) => await context.Categories.AnyAsync(x => x.Id == categoryId);
        public async Task SaveChangesAsync() => await context.SaveChangesAsync();
    }
}
