import { CartItemDto } from '../cart/cart-item.model';
import { ShippingAddressDto } from '../auth/shipping-address.model';
import { PaymentProviderDto } from './payment-provider.model';

export interface CheckoutDataDto {
    cartItems: CartItemDto[];
    address: ShippingAddressDto;
    providers: PaymentProviderDto[];
}
