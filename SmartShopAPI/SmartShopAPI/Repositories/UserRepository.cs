using Microsoft.EntityFrameworkCore;

using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Repositories
{
    public class UserRepository(SmartShopDbContext context) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email) =>
            await context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

        public async Task<int?> GetAddressIdAsync(int userId) =>
            await context.Users.Where(u => u.Id == userId)
                .SelectMany(u => u.Addresses.Where(a => a.IsDefault))
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

        public async Task<string?> GetEmailByIdAsync(int userId) =>
            await context.Users.Where(u => u.Id == userId).Select(u => u.Email).SingleOrDefaultAsync();

        public async Task AddAsync(User user) =>
            await context.Users.AddAsync(user);

        public async Task<bool> EmailExistsAsync(string email) =>
            await context.Users.AnyAsync(u => u.Email == email);

        public async Task SaveChangesAsync() => await context.SaveChangesAsync();

        public async Task<ShippingAddressDto?> GetShippingAddressAsync(int userId) =>
            await context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Addresses.Where(a => a.IsDefault))
                .Select(a => new ShippingAddressDto
                {
                    FirstName = a.User.FirstName,
                    LastName = a.User.LastName,
                    Street = a.Street,
                    City = a.City,
                    PostalCode = a.PostalCode
                })
                .FirstOrDefaultAsync();

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
            await context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
