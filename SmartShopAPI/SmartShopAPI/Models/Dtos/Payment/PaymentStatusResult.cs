namespace SmartShopAPI.Models.Dtos.Payment
{
    public class PaymentStatusResult
    {
        public bool Success { get; set; }
        public string ProviderOrderId { get; set; }
        public int OrderId { get; set; }
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
