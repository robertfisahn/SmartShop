using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.User;
namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<int?> GetAddressIdAsync(int userId);
        Task<string?> GetEmailByIdAsync(int userId);
        Task AddAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task SaveChangesAsync();
        Task<ShippingAddressDto?> GetShippingAddressAsync(int userId);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}
