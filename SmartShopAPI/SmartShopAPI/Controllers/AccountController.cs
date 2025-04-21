using Microsoft.AspNetCore.Mvc;

using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        [HttpPost("registration")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Register([FromBody] RegisterUserDto dto)
        {
            await accountService.RegisterUser(dto);
            return Ok();
        }
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ResponseDto>> Login([FromBody] LoginDto dto)
        {
            return Ok(await accountService.GenerateJwt(dto));
        }
    }
}
