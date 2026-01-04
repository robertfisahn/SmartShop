using SmartShopAPI.Models.Dtos.Payment;

public interface IPaymentService
{
    Task<PaymentInitResult> StartPaymentAsync(int orderId, decimal amount, string providerName);
    Task HandleWebhookAsync(string providerName, string providerOrderId);
    List<PaymentProviderDto> GetAvailableProviders();
}
