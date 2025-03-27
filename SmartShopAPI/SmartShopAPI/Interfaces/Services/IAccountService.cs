using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IAccountService
    {
        Task<bool> EmailExistsAsync(string email);
        Task RegisterUser(RegisterUserDto dto);
        Task<ResponseDto> GenerateJwt(LoginDto dto);
        Task<int> GetAddressId(int userId);
    }
}