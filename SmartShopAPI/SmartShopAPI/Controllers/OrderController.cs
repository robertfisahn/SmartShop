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
        public ActionResult CreateOrder()
        {
            int id = orderService.AddOrder(userContextService.GetUserId());
            return CreatedAtAction(nameof(GetOrderById), new { orderId = id }, new { orderId = id });
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult<Order> GetOrderById([FromRoute]int orderId)
        {
            var order = orderService.GetById(orderId);

            return Ok(order);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Order>> GetUserOrders()
        {
            var orders = orderService.GetUserOrders(userContextService.GetUserId());
            return Ok(orders);
        }

    }
}
