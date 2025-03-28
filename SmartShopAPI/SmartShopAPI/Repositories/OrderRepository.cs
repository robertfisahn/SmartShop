using Microsoft.EntityFrameworkCore;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces.Repositories;

namespace SmartShopAPI.Repositories
{
    public class OrderRepository(SmartShopDbContext context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId) =>
            await context.Orders.Where(u => u.UserId == userId).ToListAsync();
        public async Task<Order?> GetAsync(int orderId) =>
            await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        public async Task AddAsync(Order order) => await context.Orders.AddAsync(order);

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
