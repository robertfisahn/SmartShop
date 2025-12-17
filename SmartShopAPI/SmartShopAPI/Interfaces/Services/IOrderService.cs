using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.Order;
using SmartShopAPI.Models.Dtos.Payment;

namespace SmartShopAPI.Interfaces.Services
{
    public interface IOrderService
    {
        Task<PlaceOrderResponse> PlaceOrder(int userId, string provider);
        Task<OrderDto> GetById(int orderId, int userId);
        Task<IEnumerable<OrderDto>> GetUserOrders(int userId);
        Task<CheckoutDataDto> GetCheckoutData(int userId);
    }
}
