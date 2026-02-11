import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CheckoutDataDto } from './checkout-data.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class CheckoutService {
    private apiUrl = `${environment.apiUrl}/api/order/checkout-data`;

    private cachedData: CheckoutDataDto | null = null;

    constructor(private http: HttpClient) { }

    loadCheckoutData(): Observable<CheckoutDataDto> {
        return this.http.get<CheckoutDataDto>(this.apiUrl);
    }

    setData(data: CheckoutDataDto) {
        this.cachedData = data;
    }

    getData(): CheckoutDataDto | null {
        return this.cachedData;
    }
}
