using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SmartShopAPI.Authorization;
using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.CartItem;

namespace SmartShopAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController(ICartService cartService, IAuthorizationService authorizationService, IUserContextService userContextService) : ControllerBase
    {

        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
        {
            return Ok(await cartService.GetCart(userContextService.GetUserId()));
        }

        [HttpPost("add")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> AddCartItem([FromBody] CreateCartItemDto dto)
        {
            var cartItemId = await cartService.AddCartItem(dto, userContextService.GetUserId());
            return CreatedAtAction(nameof(GetCart), new { cartItemId }, null);
        }

        [HttpDelete("delete/{cartItemId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteCartItem([FromRoute] int cartItemId)
        {

            var cartItem = await cartService.GetCartItemById(cartItemId);
            var authorizationResult = await authorizationService.AuthorizeAsync(userContextService.User, cartItem,
                new ResourceOperationRequirement(ResourceOperation.Delete));
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Authorization failed");
            }
            await cartService.DeleteCartItem(cartItemId);
            return NoContent();
        }

        [HttpPut("update/{cartItemId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateCartItem([FromRoute] int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var cartItem = await cartService.GetCartItemById(cartItemId);
            var authorizationResult = await authorizationService.AuthorizeAsync(userContextService.User, cartItem,
                new ResourceOperationRequirement(ResourceOperation.Update));
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Authorization failed");
            }
            await cartService.UpdateCartItem(cartItemId, dto);
            return Ok();
        }

        [HttpDelete("clear")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ClearCart()
        {
            await cartService.ClearCart(userContextService.GetUserId());
            return NoContent();
        }
    }
}
