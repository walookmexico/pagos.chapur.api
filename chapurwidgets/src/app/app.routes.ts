
import { Routes, RouterModule } from '@angular/router';
import { PaycreditcardComponent } from './components/paycreditcard/paycreditcard.component';
import { PaysComponent } from './components/pays/pays.component';

const APP_ROUTES: Routes = [
  { path: 'paysDetails', component: PaysComponent },
  { path: 'paycreditcard', component: PaycreditcardComponent },
  { path: '**', pathMatch: 'full', redirectTo: 'pagoTarjeta' }
];

export const APP_ROUTING = RouterModule.forRoot(APP_ROUTES);
