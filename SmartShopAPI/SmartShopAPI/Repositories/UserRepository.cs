using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class UserRepository(SmartShopDbContext context) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email) =>
            await context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        public async Task<int?> GetAddressIdAsync(int userId) =>
            await context.Users.Where(u => u.Id == userId).Select(u => u.AddressId).SingleOrDefaultAsync();
        public async Task<string?> GetEmailByIdAsync(int userId) =>
            await context.Users.Where(u => u.Id == userId).Select(u => u.Email).SingleOrDefaultAsync();
        public async Task AddAsync(User user) =>
            await context.Users.AddAsync(user);
        public async Task<bool> EmailExistsAsync(string email)
        => await context.Users.AnyAsync(u => u.Email == email);
        public async Task SaveChangesAsync() => await context.SaveChangesAsync();

        public async Task<User?> GetShippingAddress(int userId) =>
            await context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
