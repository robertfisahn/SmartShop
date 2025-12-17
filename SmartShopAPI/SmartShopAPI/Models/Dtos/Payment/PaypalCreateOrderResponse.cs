namespace SmartShopAPI.Models.Dtos.Payment
{
    public class PayPalCreateOrderResponse
    {
        public string id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public List<PayPalLink> links { get; set; } = new();
    }

    public class PayPalLink
    {
        public string href { get; set; } = string.Empty;
        public string rel { get; set; } = string.Empty;
        public string method { get; set; } = string.Empty;
    }

}
