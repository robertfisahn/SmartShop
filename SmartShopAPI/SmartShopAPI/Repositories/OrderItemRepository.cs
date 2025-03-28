using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class OrderItemRepository(SmartShopDbContext context) : IOrderItemRepository
    {
        public async Task AddOrderItemsAsync(List<OrderItem> orderItems) => await context.OrderItems.AddRangeAsync(orderItems);
    }
}
