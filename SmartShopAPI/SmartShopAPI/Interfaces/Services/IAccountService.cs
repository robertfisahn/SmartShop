using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        ResponseDto GenerateJwt(LoginDto dto);
        int GetUserAddressId(int userId);
    }
}