import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { RegisterUserDto } from '../register-user.model';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css'],
    standalone: false
})
export class RegisterComponent {

    constructor(private authService: AuthService, private router: Router) { }

    onRegister(form: NgForm) {
        if (form.invalid) {
            return;
        }

        const user: RegisterUserDto = form.value;

        this.authService.register(user).subscribe({
            next: (response) => {
                this.router.navigate(['/login']);
            },
            error: (err) => {
                console.error('Registration failed', err);
            }
        });
    }

}
