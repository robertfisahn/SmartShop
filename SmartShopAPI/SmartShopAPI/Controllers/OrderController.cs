using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Services;

namespace SmartShopAPI.Controllers
{
    [Route("/api/order")]
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
        public async Task<ActionResult<Order>> GetById([FromRoute]int orderId)
        {
            return Ok(await orderService.GetById(orderId));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders()
        {
            return Ok(await orderService.GetUserOrders(userContextService.GetUserId()));
        }

    }
}
