import { Component, OnInit } from '@angular/core';
import { CartItemDto } from '../../cart/cart-item.model';
import { CheckoutService } from '../../checkout/checkout.service';
import { CheckoutDataDto } from '../../checkout/checkout-data.model';
import { ShippingAddressDto } from '../../auth/shipping-address.model';
import { PaymentProviderDto } from '../../checkout/payment-provider.model';
import { OrderService } from '../order.service';
import { PlaceOrderResponse } from '../place-order-response';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-order-confirmation',
    templateUrl: './order-confirmation.component.html',
    styleUrls: ['./order-confirmation.component.css'],
    standalone: false
})
export class OrderConfirmationComponent implements OnInit {

    cartItems: CartItemDto[] = [];
    userAddress!: ShippingAddressDto;
    providers: PaymentProviderDto[] = [];

    selectedProvider = 'PayPal';
    totalAmount = 0;

    private apiUrl = `${environment.apiUrl}`;

    constructor(
        private checkoutService: CheckoutService,
        private orderService: OrderService
    ) { }

    ngOnInit(): void {
        const cached = this.checkoutService.getData();

        if (!cached) {
            this.checkoutService.loadCheckoutData().subscribe({
                next: (data) => this.populate(data),
                error: (err) => console.error('Checkout fallback failed:', err)
            });
            return;
        }

        this.populate(cached);
    }

    populate(data: CheckoutDataDto): void {
        this.cartItems = data.cartItems;
        this.userAddress = data.address;
        this.providers = data.providers;

        this.totalAmount = this.cartItems
            .reduce((sum, item) => sum + item.productPrice * item.quantity, 0);
    }

    getProductImageUrl(path: string): string {
        return `${this.apiUrl}/${path}`;
    }

    placeOrder(): void {
        this.orderService.placeOrder(this.selectedProvider).subscribe({
            next: (res: PlaceOrderResponse) => {
                window.location.href = res.paymentUrl;
            },
            error: (err) => console.error('Order failed:', err)
        });
    }
}
