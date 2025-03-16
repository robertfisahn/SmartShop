using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShopAPI.Authorization;
using SmartShopAPI.Entities;
using SmartShopAPI.Exceptions;
using SmartShopAPI.Interfaces;
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
        public ActionResult<IEnumerable<CartItem>> GetCart() {
            var cartItems = cartService.GetUserCart(userContextService.GetUserId());
            return Ok(cartItems);
        }

        [HttpPost("add")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult AddItem([FromBody]CreateCartItemDto dto)
        {
            var cartItemId = cartService.AddCartItem(dto, userContextService.GetUserId());
            return Created($"api/cart/{cartItemId}", null);
        }

        [HttpDelete("delete/{cartItemId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult DeleteItem([FromRoute]int cartItemId) {

            var cartItem = cartService.GetCartItem(cartItemId);
            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, cartItem,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Authorization failed");
            }
                cartService.DeleteCartItem(cartItemId);
            return NoContent();
        }

        [HttpPut("update/{cartItemId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult UpdateItem([FromRoute]int cartItemId, [FromBody]UpdateCartItemDto dto)
        {
            var cartItem = cartService.GetCartItem(cartItemId);
            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, cartItem,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Authorization failed");
            }
            cartService.UpdateCartItem(cartItemId, dto);
            return Ok();
        }

        [HttpDelete("clear")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult ClearCart()
        {
            cartService.ClearCartItems(userContextService.GetUserId());
            return NoContent();
        }
    }
}
