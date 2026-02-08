using SmartShopAPI.Models.Dtos.Payment;

public interface IPaymentProvider
{
    string Name { get; }
    Task<PaymentInitResult> CreatePaymentAsync(int orderId, decimal amount);
    Task<PaymentStatusResult> VerifyPaymentAsync(string paymentReference);
}
