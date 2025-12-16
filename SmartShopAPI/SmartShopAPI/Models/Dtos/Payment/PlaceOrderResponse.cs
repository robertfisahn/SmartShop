namespace SmartShopAPI.Models.Dtos.Payment
{
    public class PlaceOrderResponse
    {
        public int OrderId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
