using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class OrderRepository(SmartShopDbContext context) : IOrderRepository
    {
        public IEnumerable<Order> GetUserOrders(int userId) =>
            context.Orders.Where(u => u.UserId == userId).ToList();
        public Order? GetOrder(int orderId) =>
            context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Address)
            .FirstOrDefault(o => o.Id == orderId);
        public void AddOrder(Order order) => context.Orders.Add(order);

        public void AddOrderItems(IEnumerable<OrderItem> orderItems) => context.OrderItems.AddRange(orderItems);

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
