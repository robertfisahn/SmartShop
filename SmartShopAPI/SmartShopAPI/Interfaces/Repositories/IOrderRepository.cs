using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IOrderRepository
    {

        IEnumerable<Order> GetUserOrders(int userId);
        Order? GetOrder(int orderId);
        void AddOrder(Order order);
        void SaveChanges();
    }
}
