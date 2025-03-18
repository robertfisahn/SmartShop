using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Services;
using SmartShopAPI.Models;

namespace SmartShopAPI.Controllers
{
    [Route("/api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService) 
        {
            _orderService = orderService;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult Create()
        {
            int id = _orderService.Create();
            return CreatedAtAction(nameof(GetById), new { orderId = id }, new { orderId = id });
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public ActionResult<Order> GetById([FromRoute]int orderId)
        {
            var order = _orderService.GetById(orderId);
            return Ok(order);
        }
    }
}
