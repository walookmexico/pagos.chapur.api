import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PersistenceModule } from 'angular-persistence';
import { HttpModule, Http, XHRBackend, RequestOptions } from '@angular/http';

import { APP_ROUTING } from './app.routes';
import { MorrisJsModule } from 'angular-morris-js';

import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { LoginComponent } from './components/login/login.component';
import { UsersComponent } from './components/users/users.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AlertbarComponent } from './components/alertbar/alertbar.component';
import { LoaderProcessComponent } from './components/loader-process/loader-process.component';
import { DialogsComponent } from './components/dialogs/dialogs.component';
import { DialogsAlertsComponent } from './components/dialogs-alerts/dialogs-alerts.component';
import { ForgotpasswordComponent } from './components/forgotpassword/forgotpassword.component';

import { MaterialModule  } from './app.material';

import { ChartsModule } from 'ng2-charts';
import 'chart.piecelabel.js';

/* SERVICES */
import { StorageService } from './services/storage.service';
import { TransactionsService } from './services/transactions.service';
import { UsersService } from './services/users.service';
import { AuthGuardService } from './services/auth-guard.service';
import { HttpService } from './services/http.service';
import { UtilitiesService } from './services/utilities.service';
import { CatalogsService } from './services/catalogs.service';

/* DIALOGS */
import { ChangepassworddialogComponent } from './components/changepassworddialog/changepassworddialog.component';
import { MAT_DATE_LOCALE } from '@angular/material';
import { DialogsTableComponent } from './components/dialogs-table/dialogs-table.component';


/* PIPE */
import { SumPipe } from './pipes/sum.pipe';
import { EstatuspiorpiPipe } from './pipes/estatuspiorpi.pipe';
import { EstatustransaccionPipe } from './pipes/estatustransaccion.pipe';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    LoginComponent,
    UsersComponent,
    DashboardComponent,
    AlertbarComponent,
    LoaderProcessComponent,
    DialogsComponent,
    DialogsAlertsComponent,
    ForgotpasswordComponent,
    ChangepassworddialogComponent,
    DialogsTableComponent,
    SumPipe,
    EstatuspiorpiPipe,
    EstatustransaccionPipe
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    APP_ROUTING,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    MorrisJsModule,
    PersistenceModule,
    HttpModule,
    ChartsModule
  ],
  entryComponents: [
    DialogsComponent,
    DialogsAlertsComponent,
    ChangepassworddialogComponent,
    DialogsTableComponent
  ],
  providers: [
    {
      provide: HttpService,
      useFactory: (
        backend: XHRBackend,
        options: RequestOptions,
        storageService: StorageService) => {
        return new HttpService(backend, options, storageService);
      },
      deps: [XHRBackend, RequestOptions, StorageService]
    },
    { provide: MAT_DATE_LOCALE, useValue: 'es-Mx' },
    StorageService,
    TransactionsService,
    UsersService,
    AuthGuardService,
    UtilitiesService,
    CatalogsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }


