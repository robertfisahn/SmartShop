using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrder(int userId);
        Task<Order> GetById(int id);
        Task<IEnumerable<Order>> GetUserOrders(int userId);
    }
}