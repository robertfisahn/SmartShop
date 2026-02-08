using SmartShopAPI.Models.Dtos.Payment;

namespace SmartShopAPI.Interfaces.Services.Core
{
    public interface IPaymentService
    {
        Task<PaymentInitResult> StartPaymentAsync(int orderId, decimal amount, string providerName);
        Task HandleWebhookAsync(string providerName, string paymentReference);
        List<PaymentProviderDto> GetAvailableProviders();
    }
}
