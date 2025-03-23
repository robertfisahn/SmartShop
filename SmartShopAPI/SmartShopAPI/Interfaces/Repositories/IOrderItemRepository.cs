using SmartShopAPI.Entities;

namespace SmartShopAPI.Interfaces.Repositories
{
    public interface IOrderItemRepository
    {
        void AddOrderItems(IEnumerable<OrderItem> orderItems);
    }
}
