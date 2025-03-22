using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IOrderService
    {
        int AddOrder(int userId);
        Order GetById(int id);
        IEnumerable<Order> GetUserOrders(int userId);
    }
}