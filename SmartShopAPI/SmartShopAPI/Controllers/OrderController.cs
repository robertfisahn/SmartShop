using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models.Dtos.Order;

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
        public async Task<ActionResult> Create()
        {
            var id = await orderService.PlaceOrder(userContextService.GetUserId());
            return CreatedAtAction(nameof(GetById), new { orderId = id }, new { orderId = id });
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

    }
}
