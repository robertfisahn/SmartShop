import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { RegisterUserDto } from './register-user.model';
import { CartService } from '../cart/cart.service';
import { environment } from 'src/environments/environment';
import { jwtDecode } from 'jwt-decode';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = `${environment.apiUrl}/api/auth`;

    constructor(private http: HttpClient, private cartService: CartService) { }

    login(email: string, password: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/login`, { email, password }).pipe(
            tap(response => {
                if (response && response.accessToken) {
                    sessionStorage.setItem('token', response.accessToken);
                    if (response.refreshToken) {
                        sessionStorage.setItem('refreshToken', response.refreshToken);
                    }
                    this.cartService.updateCartCount();
                }
            })
        );
    }

    register(user: RegisterUserDto): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/register`, user);
    }

    logout(): void {
        sessionStorage.removeItem('token');
    }

    isLoggedIn(): boolean {
        return !!sessionStorage.getItem('token');
    }

    isAdmin() {
        const token = sessionStorage.getItem('token');
        if (token) {
            const decodedToken: any = jwtDecode(token);
            const userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
            return userRole === 'Admin';
        }
        return false;
    }

    getUserEmail(): string | null {
        const token = sessionStorage.getItem('token');
        if (token) {
            const decodedToken: any = jwtDecode(token);
            return decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || null;
        }
        return null;
    }
}
