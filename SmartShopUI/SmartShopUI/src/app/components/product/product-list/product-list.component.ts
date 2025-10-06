import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../services/product/product.service';
import { ProductDto } from '../../../models/product.dto';
import { CartService } from '../../../services/cart/cart.service';
import { AccountService } from '../../../services/account/account.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-product',
    templateUrl: './product-list.component.html',
    styleUrls: ['./product-list.component.css'],
    standalone: false
})
export class ProductListComponent implements OnInit {
  products: ProductDto[] = [];
  showSuccessMessage = false;
  cartItemCount = 0;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    public accountService: AccountService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.getProducts();
  }

  getProducts(): void {
    this.productService.getAllProducts().subscribe((data: ProductDto[]) => {
      this.products = data;
    });
  }

  getImageUrl(imagePath: string): string {
    return `${environment.apiUrl}/${imagePath}`;
  }

  addToCart(productId: number): void {
    if (!this.accountService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    const product = this.products.find(p => p.id === productId);
    if (!product) return;
    this.cartService.getCartItemQuantity(productId).subscribe(cartQuantity => {
      if (cartQuantity < product.stockQuantity) {
        this.cartService.addItemToCart(productId).subscribe(
          response => {
            console.log('Product added to cart', response);
            this.showSuccessMessage = true;
            setTimeout(() => this.showSuccessMessage = false, 3000);
            this.updateCartCount();
          },
          error => {
            console.error('Error adding product to cart', error);
          }
        );
      } else {
        alert('No more items in stock');
      }
    });
  }

  updateCartCount(): void {
    const userId = Number(sessionStorage.getItem('userId'));
    if (userId) {
      this.cartService.getTotalQuantity().subscribe(
        count => {
          this.cartItemCount = count;
        },
        error => {
          console.error('Error fetching cart item count', error);
        }
      );
    }
  }
}
