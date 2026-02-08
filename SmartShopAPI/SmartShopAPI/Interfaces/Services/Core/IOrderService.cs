using SmartShopAPI.Entities;
using SmartShopAPI.Models.Dtos.Order;
using SmartShopAPI.Models.Dtos.Payment;
using SmartShopAPI.Models.Enums;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface IOrderService
    {
        Task<PlaceOrderResponse> PlaceOrder(int userId, string provider);
        Task<OrderDto> GetById(int orderId, int userId);
        Task<IEnumerable<OrderDto>> GetUserOrders(int userId);
        Task<CheckoutDataDto> GetCheckoutData(int userId);
        Task UpdatePaymentStatus(int orderId, string paymentReference, PaymentStatus status);
        Task UpdatePaymentStatusByReference(string paymentReference, PaymentStatus status);
    }
}
