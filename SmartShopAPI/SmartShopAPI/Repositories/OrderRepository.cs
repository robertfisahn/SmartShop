using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class OrderRepository(SmartShopDbContext context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId) =>
            await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .Include(o => o.Address)
            .Where(o => o.UserId == userId)
            .ToListAsync();
        public async Task<Order?> GetAsync(int orderId, int userId) =>
            await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        public async Task AddAsync(Order order) => await context.Orders.AddAsync(order);
    }
}
