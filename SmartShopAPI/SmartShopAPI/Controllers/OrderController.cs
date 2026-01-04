using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Interfaces.Services.Infrastructure;
using SmartShopAPI.Models.Dtos.Order;
using SmartShopAPI.Models.Dtos.Payment;

namespace SmartShopAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController(IOrderService orderService, IUserContextService userContextService) : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PlaceOrderResponse>> Create([FromBody] PlaceOrderRequest request)
        {
            var result = await orderService.PlaceOrder(
                userContextService.GetUserId(),
                request.Provider
            );

            return Ok(result);
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> GetById([FromRoute] int orderId)
        {
            return Ok(await orderService.GetById(orderId, userContextService.GetUserId()));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
        {
            return Ok(await orderService.GetUserOrders(userContextService.GetUserId()));
        }

        [HttpGet("checkout-data")]
        public async Task<ActionResult<CheckoutDataDto>> GetCheckoutData()
        {
            var result = await orderService.GetCheckoutData(userContextService.GetUserId());
            return Ok(result);
        }
    }
}
