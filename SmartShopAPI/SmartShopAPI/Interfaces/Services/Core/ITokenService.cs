using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
