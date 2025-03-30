import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AccountService } from './services/account/account.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

import { AppComponent } from './app.component';
import { ProductListComponent } from './components/product/product-list/product-list.component';
import { HomeComponent } from './home/home.component';
import { ProductDetailsComponent } from './components/product/product-details/product-details.component';
import { AccountRegisterComponent } from './components/account/account-register/account-register.component';
import { AccountLoginComponent } from './components/account/account-login/account-login.component';
import { AccountDetailsComponent } from './components/account/account-details/account-details.component';
import { CartComponent } from './components/cart/cart.component';
import { OrderCreateComponent } from './components/order/order-create/order-create.component';
import { OrderConfirmationComponent } from './components/order/order-confirmation/order-confirmation.component';
import { OrderDetailsComponent } from './components/order/order-details/order-details.component';
import { ProductCreateComponent } from './components/product/product-create/product-create.component';
import { ProductUpdateComponent } from './components/product/product-update/product-update.component';
import { ProductDeleteComponent } from './components/product/product-delete/product-delete.component';
import { OrderHistoryComponent } from './components/order/order-history/order-history.component';

@NgModule({ declarations: [
        AppComponent,
        HomeComponent,
        ProductListComponent,
        ProductDetailsComponent,
        AccountRegisterComponent,
        AccountLoginComponent,
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
  constructor(public accountService: AccountService) { }
}
