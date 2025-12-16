import { CartItemDto } from '../cart/cart-item.dto';
import { ShippingAddressDto } from '../user/shipping-address.dto';
import { PaymentProviderDto } from '../payment/payment-provider.dto';

export interface CheckoutDataDto {
  cartItems: CartItemDto[];
  address: ShippingAddressDto;
  providers: PaymentProviderDto[];
}
