import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Component } from '@angular/core';
import { Routes,  RouterModule, Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MaterialModule  } from './app.material';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { MAT_DATE_LOCALE } from '@angular/material';

import { APP_ROUTING } from './app.routes';

import { TokenService } from './services/token.service';
import { PayService } from './services/pay.service';
import { ModalsService } from './services/modals.service';

import { AppComponent } from './app.component';
import { PaycreditcardComponent } from './components/paycreditcard/paycreditcard.component';
import { PaysComponent } from './components/pays/pays.component';
import { LoaderProcessComponent } from './components/loader-process/loader-process.component';
import { DialogsTokenComponent } from './components/dialogs-token/dialogs-token.component';
import { DialogsAlertsComponent } from './components/dialogs-alerts/dialogs-alerts.component';
import { DialogsAlertsSuccesComponent } from './components/dialogs-alerts-succes/dialogs-alerts-succes.component';



@NgModule({
  declarations: [
    AppComponent,
    PaycreditcardComponent,
    PaysComponent,
    LoaderProcessComponent,
    DialogsTokenComponent,
    DialogsAlertsComponent,
    DialogsAlertsSuccesComponent
  ],
  imports: [
    MaterialModule,
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    APP_ROUTING,
    BrowserAnimationsModule
  ],
  entryComponents: [
    DialogsTokenComponent,
    DialogsAlertsComponent,
    DialogsAlertsSuccesComponent
  ],
  providers: [TokenService, PayService, ModalsService,
    {
      provide: MAT_DATE_LOCALE, useValue: 'es-Mx'
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
