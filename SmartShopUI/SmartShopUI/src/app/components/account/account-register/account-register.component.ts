import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { RegisterUserDto } from '../../../models/registerUser.dto';
import { Router } from '@angular/router';
import { AccountService } from '../../../services/account/account.service';

@Component({
    selector: 'app-account-register',
    templateUrl: './account-register.component.html',
    styleUrls: ['./account-register.component.css'],
    standalone: false
})
export class AccountRegisterComponent {

  constructor(private accountService: AccountService, private router: Router) { }

  onRegister(form: NgForm) {
    if (form.invalid) {
      return;
    }

    const user: RegisterUserDto = form.value;

    this.accountService.register(user).subscribe({
      next: (response) => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Registration failed', err);
      }
    });
  }

}
