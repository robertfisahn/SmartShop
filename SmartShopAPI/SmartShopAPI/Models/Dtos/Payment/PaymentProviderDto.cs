namespace SmartShopAPI.Models.Dtos.Payment
{
    public class PaymentProviderDto
    {
        public string Name { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string LogoUrl { get; set; } = default!;
        public bool IsEnabled { get; set; }
    }
}
