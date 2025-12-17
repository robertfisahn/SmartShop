using SmartShopAPI.Models.Dtos.Payment;

namespace SmartShopAPI.Models.Settings
{
    public class PaymentsSettings
    {
        public List<PaymentProviderDto> Providers { get; set; } = [];
    }
}
