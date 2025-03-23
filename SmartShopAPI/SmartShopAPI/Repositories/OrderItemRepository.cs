using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class OrderItemRepository(SmartShopDbContext context) : IOrderItemRepository
    {
        public void AddOrderItems(IEnumerable<OrderItem> orderItems) => context.OrderItems.AddRange(orderItems);
    }
}
