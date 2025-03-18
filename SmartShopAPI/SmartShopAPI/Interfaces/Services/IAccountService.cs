using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        public ResponseDto GenerateJwt(LoginDto dto);
    }
}