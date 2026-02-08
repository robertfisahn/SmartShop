using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Services.Core;

namespace SmartShopAPI.Services.Core
{
    public class TokenService(AuthenticationSettings settings) : ITokenService
    {
        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, user.Role.Name),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(60);

            var token = new JwtSecurityToken(
                settings.JwtIssuer,
                settings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
