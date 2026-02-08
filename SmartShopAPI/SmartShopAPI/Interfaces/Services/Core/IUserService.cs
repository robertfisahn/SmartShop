using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface IUserService
    {
        Task<string?> GetEmailByIdAsync(int userId);
        Task<ShippingAddressDto?> GetShippingAddressAsync(int userId);
    }
}
