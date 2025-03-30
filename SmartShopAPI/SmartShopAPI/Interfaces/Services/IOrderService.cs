using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.Order;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrder(int userId);
        Task<Order> GetById(int id);
        Task<IEnumerable<OrderDto>> GetUserOrders(int userId);
    }
}