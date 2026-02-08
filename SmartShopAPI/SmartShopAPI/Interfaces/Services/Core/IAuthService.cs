using SmartShopAPI.Models.Dtos.Auth;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface IAuthService
    {
        Task Register(RegisterUserDto dto);
        Task<AuthResponseDto> Login(LoginDto dto);
        Task<AuthResponseDto> RefreshToken(string refreshToken);
    }
}
