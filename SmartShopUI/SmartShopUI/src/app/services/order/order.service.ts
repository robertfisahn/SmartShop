import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CartService } from '../cart/cart.service';
import { Order } from '../../models/order.dto';
import { PlaceOrderResponse } from '../../models/order/place-order-response';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/api/order`;

  constructor(private http: HttpClient, private cartService: CartService) { }

  createOrder(): Observable<number> {
    const token = sessionStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.post<any>(this.apiUrl, {}, { headers }).pipe(
      map(response => {
        this.cartService.updateCartCount();
        if (response && response.orderId) {
          return response.orderId;
        } else {
          throw new Error('OrderId missing in response');
        }
      })
    );
  }

  getOrderById(orderId: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${orderId}`);
  }

  getUserOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}`);
  }

  placeOrder(provider: string): Observable<PlaceOrderResponse> {
    return this.http.post<PlaceOrderResponse>(
      `${this.apiUrl}`,
      { provider }
    );
  }

}
