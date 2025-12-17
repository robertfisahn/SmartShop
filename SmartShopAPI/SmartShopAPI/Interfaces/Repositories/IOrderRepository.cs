using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetAsync(int orderId, int userId);
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(int orderId);
        Task<Order?> GetByProviderOrderIdAsync(string providerOrderId);
        Task UpdateAsync(Order order);
    }
}
