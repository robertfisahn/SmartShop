import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../../services/account/account.service';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'app-account-login',
    templateUrl: './account-login.component.html',
    styleUrls: ['./account-login.component.css'],
    standalone: false
})
export class AccountLoginComponent {
  constructor(private accountService: AccountService, private router: Router) { }

  onLogin(form: NgForm) {
    if (form.invalid) {
      return;
    }

    const { email, password } = form.value;

    this.accountService.login(email, password).subscribe({
      next: (response) => {
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Login failed', err);
      }
    });
  }
}
