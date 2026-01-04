using Microsoft.Extensions.Options;

using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services.Integrations;
using SmartShopAPI.Models.Dtos.Payment;
using SmartShopAPI.Models.Enums;
using SmartShopAPI.Models.Settings;

namespace SmartShopAPI.Services.Core
{
    public class PaymentService(IPaymentProviderFactory _factory, IOrderRepository _orders, IOptions<PaymentsSettings> _config) : IPaymentService
    {
        public async Task<PaymentInitResult> StartPaymentAsync(int orderId, decimal amount, string providerName)
        {
            var provider = _factory.GetProvider(providerName);
            var payment = await provider.CreatePaymentAsync(orderId, amount);

            var order = await _orders.GetByIdAsync(orderId);
            order.PaymentProviderOrderId = payment.ProviderOrderId;
            order.PaymentStatus = PaymentStatus.Pending;

            await _orders.UpdateAsync(order);
            return payment;
        }

        public async Task HandleWebhookAsync(string providerName, string providerOrderId)
        {
            var provider = _factory.GetProvider(providerName);
            var status = await provider.VerifyPaymentAsync(providerOrderId);

            var order = await _orders.GetByProviderOrderIdAsync(providerOrderId);
            if (order == null)
                throw new Exception("Order not found for webhook");

            if (status.Success)
                order.PaymentStatus = PaymentStatus.Paid;
            else
                order.PaymentStatus = PaymentStatus.Failed;

            await _orders.UpdateAsync(order);
        }

        public List<PaymentProviderDto> GetAvailableProviders()
        {
            return _config.Value.Providers
                .Where(p => p.IsEnabled)
                .ToList();
        }
    }
}
