import { Component, HostListener, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './features/auth/auth.service';
import { CartService } from './features/cart/cart.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: false
})
export class AppComponent implements OnInit {
  title = 'SmartShopUI';
  searchQuery: string = '';
  dropdownOpen = false;
  cartItemCount = 0;
  productMenuOpen = false;
  isLoggedIn = false;
  isAdminRole = false;
  productId: number | null = null;
  showModal: boolean = false;
  operation: 'update' | 'delete' = 'update';

  constructor(private router: Router, public authService: AuthService, private cartService: CartService) { }

  ngOnInit(): void {
    this.cartService.getCartItemCount().subscribe(count => {
      this.cartItemCount = count;
    });
    this.isLoggedIn = this.authService.isLoggedIn();
    this.isAdminRole = this.authService.isAdmin();
  }

  onSearch(): void {
    if (this.searchQuery) {
      this.router.navigate(['/search'], { queryParams: { q: this.searchQuery } });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: Event) {
    const clickedInside = (event.target as HTMLElement).closest('.dropdown');
    if (!clickedInside) {
      this.dropdownOpen = false;
    }
  }

  toggleProductMenu() {
    this.productMenuOpen = !this.productMenuOpen;
  }

  openModal(operation: 'update' | 'delete') {
    this.operation = operation;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  onConfirm() {
    if (this.productId) {
      this.closeModal();
      if (this.operation === 'update') {
        this.router.navigate([`/product-update/${this.productId}`]);
      } else if (this.operation === 'delete') {
        this.router.navigate([`/product-delete/${this.productId}`]);
      }
    }
  }

}
