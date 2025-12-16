using Microsoft.EntityFrameworkCore;

using SendGrid.Helpers.Mail;

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

        public Task<Order?> GetByIdAsync(int orderId) =>
            context.Orders
            .Where(o => o.Id == orderId)
            .FirstOrDefaultAsync();

        public Task<Order?> GetByProviderOrderIdAsync(string providerOrderId) =>
            context.Orders
            .Where(o => o.PaymentProviderOrderId == providerOrderId)
            .FirstOrDefaultAsync();
        public async Task AddAsync(Order order) => await context.Orders.AddAsync(order);

        public async Task UpdateAsync(Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
    }
}
