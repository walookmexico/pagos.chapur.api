import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { UsersComponent } from './components/users/users.component';
import { ForgotpasswordComponent } from './components/forgotpassword/forgotpassword.component';

import { AuthGuardService } from './services/auth-guard.service';

const APP_ROUTES: Routes = [
  { path: '', component: LoginComponent },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent , canActivate: [ AuthGuardService ] },
  { path: 'users', component: UsersComponent , canActivate: [ AuthGuardService ] },
  { path: 'forgotpassword', component: ForgotpasswordComponent },
  // { path: '**', pathMatch: 'full', redirectTo: 'login' }
];

export const APP_ROUTING = RouterModule.forRoot(APP_ROUTES);
