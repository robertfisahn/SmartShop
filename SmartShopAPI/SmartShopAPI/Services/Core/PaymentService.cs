using Microsoft.Extensions.Options;

using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Interfaces.Services.Integrations;
using SmartShopAPI.Models.Dtos.Payment;
using SmartShopAPI.Models.Enums;
using SmartShopAPI.Models.Settings;

namespace SmartShopAPI.Services.Core
{
    public class PaymentService(IPaymentProviderFactory _factory, IOrderService orderService, IOptions<PaymentsSettings> _config) : IPaymentService
    {
        public async Task<PaymentInitResult> StartPaymentAsync(int orderId, decimal amount, string providerName)
        {
            var provider = _factory.GetProvider(providerName);
            var payment = await provider.CreatePaymentAsync(orderId, amount);

            await orderService.UpdatePaymentStatus(orderId, payment.ProviderOrderId, PaymentStatus.Pending);

            return payment;
        }

        public async Task HandleWebhookAsync(string providerName, string paymentReference)
        {
            var provider = _factory.GetProvider(providerName);
            var status = await provider.VerifyPaymentAsync(paymentReference);

            if (status.Success)
                await orderService.UpdatePaymentStatusByReference(paymentReference, PaymentStatus.Paid);
            else
                await orderService.UpdatePaymentStatusByReference(paymentReference, PaymentStatus.Failed);
        }

        public List<PaymentProviderDto> GetAvailableProviders()
        {
            return _config.Value.Providers
                .Where(p => p.IsEnabled)
                .ToList();
        }
    }
}
