using Microsoft.AspNetCore.Mvc;

using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Models.Dtos.Auth;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            await authService.Register(dto);
            return Ok();
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            return Ok(await authService.Login(dto));
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            return Ok(await authService.RefreshToken(dto.RefreshToken));
        }
    }
}
