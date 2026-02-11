import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './features/product/product-list/product-list.component';
import { HomeComponent } from './features/home/home.component';
import { ProductDetailsComponent } from './features/product/product-details/product-details.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { AccountDetailsComponent } from './features/auth/account-details/account-details.component';
import { CartComponent } from './features/cart/cart.component';
import { OrderCreateComponent } from './features/order/order-create/order-create.component';
import { OrderConfirmationComponent } from './features/order/order-confirmation/order-confirmation.component';
import { OrderDetailsComponent } from './features/order/order-details/order-details.component';
import { ProductCreateComponent } from './features/product/product-create/product-create.component';
import { ProductUpdateComponent } from './features/product/product-update/product-update.component';
import { ProductDeleteComponent } from './features/product/product-delete/product-delete.component';
import { OrderHistoryComponent } from './features/order/order-history/order-history.component';

const routes: Routes = [
  { path: 'order-history', component: OrderHistoryComponent },
  { path: 'product-delete/:productId', component: ProductDeleteComponent },
  { path: 'product-update/:productId', component: ProductUpdateComponent },
  { path: 'product-create', component: ProductCreateComponent },
  { path: 'order-details/:orderId', component: OrderDetailsComponent },
  { path: 'order-confirmation', component: OrderConfirmationComponent },
  { path: 'order-create', component: OrderCreateComponent },
  { path: 'cart', component: CartComponent },
  { path: 'account-details', component: AccountDetailsComponent },
  { path: 'registration', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'product-details/:id', component: ProductDetailsComponent },
  { path: 'product/all', component: ProductListComponent },
  { path: '', component: HomeComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
