using SmartShopAPI.Models.Dtos.CartItem;
using SmartShopAPI.Models.Dtos.Payment;
using SmartShopAPI.Models.Dtos.User;

namespace SmartShopAPI.Models.Dtos.Order
{
    public class CheckoutDataDto
    {
        public List<CartItemDto> CartItems { get; set; } = default!;
        public ShippingAddressDto Address { get; set; } = default!;
        public List<PaymentProviderDto> Providers { get; set; } = default!;
    }
}
