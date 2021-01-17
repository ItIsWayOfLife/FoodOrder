import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule }   from '@angular/forms';
import {RouterModule, Routes} from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { JwtModule } from "@auth0/angular-jwt";

import {MenuCompilationService} from '../app/service/menu-compilation.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './account/login/login.component';
import { RegisterComponent } from './account/register/register.component';
import { HeaderComponent } from './shared/header/header.component';
import { FooterComponent } from './shared/footer/footer.component';

import { AuthGuard } from './account/guards/auth-guard.service';

import { HomeComponent } from './home/home.component';
import { ProfileComponent } from './account/profile/profile.component';
import { UsersComponent } from './admin/users/users.component';
import { ProviderComponent } from './provider/provider.component';
import { UploadComponent } from './upload/upload.component';
import { NotFoundComponent } from './shared/not-found/not-found.component';
import { CatalogComponent } from './catalog/catalog.component';
import { MenuComponent } from './menu/menu.component';
import { DishComponent } from './dish/dish.component';
import { MenuDishesComponent } from './menu-dishes/menu-dishes.component';
import { CartComponent } from './cart/cart.component';
import { OrderComponent } from './order/order.component';
import { ReportComponent } from './report/report.component';
import { AboutComponent } from './about/about.component';
import { PrivacyPolicyComponent } from './privacy-policy/privacy-policy.component';
import { AboutDeliveryComponent } from './about-delivery/about-delivery.component';

export function tokenGetter() {
  return localStorage.getItem("jwt");
}

const appRoutes: Routes = [
  {   path: '', component: HomeComponent },
  {   path: 'login', component: LoginComponent},
  {   path: 'register', component: RegisterComponent},
  {   path: 'account', component: ProfileComponent, canActivate: [AuthGuard]},
  {   path: 'users', component: UsersComponent, canActivate: [AuthGuard]},
  {   path: 'providers', component: ProviderComponent},
  {   path: 'catalog/:providerId', component: CatalogComponent},
  {   path: 'menu/:providerId', component: MenuComponent},
  {   path: 'dish/:catalogId', component: DishComponent},
  {   path: 'menudishes/:menuId', component: MenuDishesComponent},
  {   path: 'cart', component: CartComponent, canActivate: [AuthGuard]},
  {   path: 'order', component: OrderComponent, canActivate: [AuthGuard]},
  {   path: 'report', component: ReportComponent, canActivate: [AuthGuard]},
  {   path: 'about', component: AboutComponent},
  {   path: 'aboutdelivery', component: AboutDeliveryComponent},
  {   path: 'privacypolicy', component: PrivacyPolicyComponent},
  {   path: '**', component: NotFoundComponent }
]

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HeaderComponent,
    FooterComponent,
    HomeComponent,
    ProfileComponent,
    UsersComponent,
    ProviderComponent,
    UploadComponent,
    NotFoundComponent,
    CatalogComponent,
    MenuComponent,
    DishComponent,
    MenuDishesComponent,
    CartComponent,
    OrderComponent,
    ReportComponent,
    AboutComponent,
    PrivacyPolicyComponent,
      AboutDeliveryComponent
   ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["localhost:44342"],
        blacklistedRoutes: []
      }
    })
],
  providers: [AuthGuard, MenuCompilationService],
  bootstrap: [AppComponent]
})
export class AppModule { }
