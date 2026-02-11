import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { ProductDto } from './product.model';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private apiUrl = `${environment.apiUrl}/api/product`;

    constructor(private http: HttpClient) { }

    getAllProducts(): Observable<ProductDto[]> {
        return this.http.get<ProductDto[]>(`${this.apiUrl}/all`);
    }

    getProductById(productId: number): Observable<ProductDto> {
        return this.http.get<ProductDto>(`${this.apiUrl}/${productId}`)
    }

    createProduct(productData: FormData): Observable<any> {
        const token = sessionStorage.getItem('token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        });
        return this.http.post<any>(`${this.apiUrl}`, productData, { headers });
    }

    updateProduct(productId: number, productData: FormData): Observable<any> {
        const token = sessionStorage.getItem('token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        });
        return this.http.put<any>(`${this.apiUrl}/${productId}`, productData, { headers });
    }

    deleteProduct(id: number): Observable<void> {
        const token = sessionStorage.getItem('token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        });
        return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
    }

    getImageUrl(imagePath: string): string {
        return `${environment.apiUrl}/${imagePath}`;
    }

    checkProductName(productName: string): Observable<boolean> {
        const token = sessionStorage.getItem('token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        });
        const params = { productName };

        return this.http.get<boolean>(`${this.apiUrl}/check`, { headers, params }).pipe(
            catchError((error) => {
                return throwError(() => new Error(error.error?.error || 'An error occurred while checking the product name.'));
            })
        );
    }
}
