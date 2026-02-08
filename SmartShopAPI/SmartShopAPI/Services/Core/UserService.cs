using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Services.Core
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<string?> GetEmailByIdAsync(int userId) =>
            await userRepository.GetEmailByIdAsync(userId);

        public async Task<ShippingAddressDto?> GetShippingAddressAsync(int userId) =>
            await userRepository.GetShippingAddressAsync(userId);
    }
}
