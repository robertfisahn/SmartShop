using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface IAccountService
    {
        Task<bool> EmailExistsAsync(string email);
        Task RegisterUser(RegisterUserDto dto);
        Task<ResponseDto> GenerateJwt(LoginDto dto);
        Task<int> GetAddressId(int userId);
        Task<string?> GetEmailByIdAsync(int userId);
        Task<ShippingAddressDto?> GetShippingAddress(int userId);
    }
}
