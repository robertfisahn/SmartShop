import { Component, OnInit } from '@angular/core';
import { CartItemDto } from './cart-item.model';
import { CartService } from './cart.service';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { OrderService } from '../order/order.service';
import { CheckoutService } from '../checkout/checkout.service';

@Component({
    selector: 'app-cart',
    templateUrl: './cart.component.html',
    styleUrls: ['./cart.component.css'],
    standalone: false
})
export class CartComponent implements OnInit {
    private apiUrl = `${environment.apiUrl}`;
    cartItems: CartItemDto[] = [];
    userId: number = 0;
    totalAmount = 0;

    constructor(
        private cartService: CartService,
        private router: Router,
        private orderService: OrderService,
        private checkoutService: CheckoutService
    ) { }

    ngOnInit(): void {
        this.userId = +sessionStorage.getItem('userId')!;

        this.cartService.getCart().subscribe(
            (items: CartItemDto[]) => {
                this.cartItems = items;
                this.calculateTotal();
            },
            (error) => {
                console.error('Error fetching cart items:', error);
            }
        );
    }

    calculateTotal(): void {
        this.totalAmount = this.cartItems.reduce((sum, item) => sum + item.productPrice * item.quantity, 0);
    }

    loadCart(): void {
        this.cartService.getCart().subscribe(
            data => {
                this.cartItems = data;
                this.calculateTotal();
            },
            error => console.error('Error fetching cart items:', error)
        );
    }

    removeItem(itemId: number): void {
        this.cartService.removeItemFromCart(itemId).subscribe(
            () => this.loadCart(),
            error => console.error('Error removing item from cart', error)
        );
    }

    updateItem(itemId: number, quantity: number): void {
        if (quantity < 1 || quantity > this.getProductStock(itemId)) {
            alert('Please enter a valid quantity between 1 and ' + this.getProductStock(itemId));
            return;
        }

        this.cartService.updateCartItem(itemId, quantity).subscribe(
            () => this.loadCart(),
            error => console.error('Error updating item in cart', error)
        );
    }

    getProductStock(itemId: number): number {
        const item = this.cartItems.find(i => i.id === itemId);
        return item ? item.productStockQuantity : 0;
    }

    getProductImageUrl(path: string): string {
        return `${this.apiUrl}/${path}`;
    }

    clearCart(): void {
        this.cartService.clearCart().subscribe(
            () => this.loadCart(),
            error => console.error('Error clearing the cart', error)
        );
    }

    navigateToOrderConfirmation(): void {
        this.checkoutService.loadCheckoutData().subscribe({
            next: (data) => {
                this.checkoutService.setData(data);
                this.router.navigate(['/order-confirmation']);
            },
            error: (err) => console.error('Error loading checkout data:', err)
        });
    }
}
