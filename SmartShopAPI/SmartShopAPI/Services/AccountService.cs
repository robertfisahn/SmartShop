using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.User;
using SmartShopAPI.Repositories;

namespace SmartShopAPI.Services
{
    public class AccountService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher,
        AuthenticationSettings authenticationSettings, IRoleService roleService) : IAccountService
    {

        public async Task RegisterUser(RegisterUserDto dto)
        {
            if (await EmailExistsAsync(dto.Email))
            {
                throw new BadRequestException("That email is already taken");
            }
            var user = mapper.Map<User>(dto);
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
            user.RoleId = await roleService.GetUserRoleIdAsync();
            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();
        }

        public async Task<ResponseDto> GenerateJwt(LoginDto dto)
        {
            var user = await userRepository.GetByEmailAsync(dto.Email) ?? throw new BadRequestException("Invalid email or password");
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid email or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
            };

            if (user.DateOfBirth.HasValue)
            {
                claims.Add(
                    new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd"))
                );
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(authenticationSettings.JwtIssuer,
                authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return new ResponseDto { Token = tokenHandler.WriteToken(token) };
        }

        public async Task<int> GetAddressId(int userId)
        {
            return await userRepository.GetAddressIdAsync(userId) ?? throw new NotFoundException("User not found");
        }

        public async Task<bool> EmailExistsAsync(string userEmail) => await userRepository.EmailExistsAsync(userEmail);

        public async Task<string?> GetEmailByIdAsync(int userId) =>
            await userRepository.GetEmailByIdAsync(userId);
    }
}
