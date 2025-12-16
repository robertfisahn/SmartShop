namespace SmartShopAPI.Models.Dtos.Payment
{
    public class PaymentInitResult
    {
        public string ProviderOrderId { get; set; }
        public string PaymentUrl { get; set; }
    }
}
