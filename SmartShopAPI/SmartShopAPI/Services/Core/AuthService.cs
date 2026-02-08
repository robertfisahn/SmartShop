using Microsoft.AspNetCore.Identity;

using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Models.Dtos.Auth;
using SmartShopAPI.Models.Dtos.User;


namespace SmartShopAPI.Services.Core
{
    public class AuthService(ITokenService tokenService, IPasswordHasher<User> passwordHasher, IUserRepository userRepository, IRoleService roleService) : IAuthService
    {
        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            var user = await userRepository.GetByEmailAsync(dto.Email)
                ?? throw new BadRequestException("Invalid email or password");

            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
                throw new BadRequestException("Invalid email or password");

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await userRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task Register(RegisterUserDto dto)
        {
            if (await userRepository.GetByEmailAsync(dto.Email) != null)
                throw new BadRequestException("That email is already taken");
            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            user.RoleId = await roleService.GetUserRoleIdAsync();
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

            user.Addresses.Add(new Address
            {
                City = dto.City,
                Street = dto.Street,
                PostalCode = dto.PostalCode,
                IsDefault = true
            });
            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> RefreshToken(string refreshToken)
        {
            var user = await userRepository.GetByRefreshTokenAsync(refreshToken)
                ?? throw new BadRequestException("Invalid refresh token");

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new BadRequestException("Refresh token expired");

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await userRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
