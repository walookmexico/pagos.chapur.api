import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Appsettings } from '../configurations/appsettings';
import { Observable } from 'rxjs/Observable';
import { map, filter } from 'rxjs/operators';


@Injectable()
export class PayService {

  constructor(private http: HttpClient) { }

  payCreditcard(data: any) {

    const urlSave = `${Appsettings.API_ENDPOINT_FULL}/payments/save`;
    const headersSave = new HttpHeaders()
    .append('Content-Type', 'application/json');

    return this.http.post( urlSave , JSON.stringify(data),  { headers: headersSave })
              .map( res => res);


  }

  payDetail(data: any) {

    const urlSave = `${Appsettings.API_ENDPOINT_FULL}/payments/detail`;
    const headersSave = new HttpHeaders()
    .append('Content-Type', 'application/json');

    return this.http.post( urlSave , JSON.stringify(data),  { headers: headersSave })
              .map( res => res);


  }

}
