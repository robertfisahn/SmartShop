import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';
import { CartItem } from '../../models/cartItem.dto.';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl =`${environment.apiUrl}/api/cart`;

  private cartItemCount = new BehaviorSubject<number>(0);
  constructor(private http: HttpClient) {
    this.initializeCartCount();
  }

  private initializeCartCount(): void {
    if (sessionStorage.getItem('token')) {
      this.getTotalQuantity().subscribe(count => {
        this.cartItemCount.next(count);
      });
    }
  }

  addItemToCart(productId: number, quantity: number = 1): Observable<any> {
    const body = { productId, quantity };
    return this.http.post(`${this.apiUrl}/add`, body).pipe(
      map(() => this.updateCartCount())
    );
  }

  getCart(): Observable<any> {
    return this.http.get(`${this.apiUrl}`);
  }

  removeItemFromCart(itemId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete/${itemId}`).pipe(
      map(() => this.updateCartCount())
    );
  }

  updateCartItem(itemId: number, quantity: number): Observable<any> {
    const body = { quantity };
    return this.http.put(`${this.apiUrl}/update/${itemId}`, body).pipe(
      map(() => this.updateCartCount())
    );
  }

  getCartItemCount(): Observable<number> {
    return this.cartItemCount.asObservable();
  }

  getTotalQuantity(): Observable<number> {
    return this.getCart().pipe(
      map((items: CartItem[]) => {
        return items.reduce((total, item) => total + item.quantity, 0);
      })
    );
  }
  
  updateCartCount(): void {
    if (sessionStorage.getItem('token')) {
      this.getTotalQuantity().subscribe(count => {
        this.cartItemCount.next(count);
      });
    }
  }

  clearCart(): Observable<any> {
    return this.http.delete(`${this.apiUrl}/clear`).pipe(
      tap(() => this.cartItemCount.next(0))
    );
  }

  getCartItemQuantity(productId: number): Observable<number> {
    return this.getCart().pipe(
      map((items: CartItem[]) => {
        const item = items.find(i => i.productId === productId);
        return item ? item.quantity : 0;
      })
    );
  }
}
