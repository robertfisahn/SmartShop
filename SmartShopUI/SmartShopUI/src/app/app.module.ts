import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AuthService } from './features/auth/auth.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './core/auth.interceptor';

import { AppComponent } from './app.component';
import { ProductListComponent } from './features/product/product-list/product-list.component';
import { HomeComponent } from './features/home/home.component';
import { ProductDetailsComponent } from './features/product/product-details/product-details.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { LoginComponent } from './features/auth/login/login.component';
import { AccountDetailsComponent } from './features/auth/account-details/account-details.component';
import { CartComponent } from './features/cart/cart.component';
import { OrderCreateComponent } from './features/order/order-create/order-create.component';
import { OrderConfirmationComponent } from './features/order/order-confirmation/order-confirmation.component';
import { OrderDetailsComponent } from './features/order/order-details/order-details.component';
import { ProductCreateComponent } from './features/product/product-create/product-create.component';
import { ProductUpdateComponent } from './features/product/product-update/product-update.component';
import { ProductDeleteComponent } from './features/product/product-delete/product-delete.component';
import { OrderHistoryComponent } from './features/order/order-history/order-history.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ProductListComponent,
    ProductDetailsComponent,
    RegisterComponent,
    LoginComponent,
    AccountDetailsComponent,
    CartComponent,
    OrderCreateComponent,
    OrderConfirmationComponent,
    OrderDetailsComponent,
    OrderHistoryComponent,
    ProductCreateComponent,
    ProductUpdateComponent,
    ProductDeleteComponent
  ],
  bootstrap: [AppComponent],
  imports: [BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    provideHttpClient(withInterceptorsFromDi()),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ]
})
export class AppModule {
  constructor(public authService: AuthService) { }
}
