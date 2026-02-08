using Microsoft.Extensions.Options;

using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Interfaces.Services.Integrations;
using SmartShopAPI.Models.Dtos.Payment;
using SmartShopAPI.Models.Settings;

namespace SmartShopAPI.Services.Core
{
    public class PaymentService(IPaymentProviderFactory _factory, IOptions<PaymentsSettings> _config) : IPaymentService
    {
        public async Task<PaymentInitResult> StartPaymentAsync(int orderId, decimal amount, string providerName)
        {
            var provider = _factory.GetProvider(providerName);
            return await provider.CreatePaymentAsync(orderId, amount);
        }

        public async Task<PaymentStatusResult> VerifyPaymentAsync(string providerName, string paymentReference)
        {
            var provider = _factory.GetProvider(providerName);
            return await provider.VerifyPaymentAsync(paymentReference);
        }

        public List<PaymentProviderDto> GetAvailableProviders()
        {
            return _config.Value.Providers
                .Where(p => p.IsEnabled)
                .ToList();
        }
    }
}

