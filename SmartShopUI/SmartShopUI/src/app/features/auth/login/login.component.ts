import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
    standalone: false
})
export class LoginComponent {
    constructor(private authService: AuthService, private router: Router) { }

    onLogin(form: NgForm) {
        if (form.invalid) {
            return;
        }

        const { email, password } = form.value;

        this.authService.login(email, password).subscribe({
            next: (response) => {
                this.router.navigate(['/']);
            },
            error: (err) => {
                console.error('Login failed', err);
            }
        });
    }
}
