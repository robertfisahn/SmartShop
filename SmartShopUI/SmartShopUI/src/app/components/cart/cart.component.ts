import { Component, OnInit } from '@angular/core';
import { CartItem } from '../../models/cartItem.dto.';
import { CartService } from '../../services/cart/cart.service';
import { ProductDto } from '../../models/product.dto';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { OrderService } from '../../services/order/order.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  private apiUrl = `${environment.apiUrl}`;
  cartItems: CartItem[] = [];
  userId: number = 0;
  totalAmount = 0;

  constructor(private cartService: CartService, private router: Router, private orderService: OrderService) { }

  ngOnInit(): void {
    this.userId = +sessionStorage.getItem('userId')!;

    this.cartService.getCart().subscribe(
      (items: CartItem[]) => {
        this.cartItems = items;
        this.calculateTotal();
      },
      (error) => {
        console.error('Error fetching cart items:', error);
      }
    );
  }

  calculateTotal(): void {
    this.totalAmount = this.cartItems.reduce((sum, item) => sum + item.product.price * item.quantity, 0);
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
    return item ? item.product.stockQuantity : 0;
  }

  getProductImageUrl(product: ProductDto): string {
    return `${this.apiUrl}/${product.imagePath}`;
  }

  clearCart(): void {
    this.cartService.clearCart().subscribe(
      () => this.loadCart(),
      error => console.error('Error clearing the cart', error)
    );
  }

  goToCreateOrder(): void {
    this.orderService.createOrder().subscribe({
      next: (orderId) => {
        console.log('Order created successfully:', orderId);
        this.router.navigate([`/order-details/${orderId}`]);
      },
      error: (err) => {
        console.error('Error creating order:', err);
      }
    });
  }
}
