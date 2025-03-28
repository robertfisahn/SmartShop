using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IOrderItemRepository
    {
        Task AddOrderItemsAsync(List<OrderItem> orderItems);
    }
}
