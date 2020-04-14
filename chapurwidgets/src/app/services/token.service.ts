import { Injectable } from '@angular/core';
import { Headers, Http, Response } from '@angular/http';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Appsettings } from '../configurations/appsettings';

import 'rxjs/Rx';

@Injectable()
export class TokenService {

  tokenUrl = `${Appsettings.API_ENDPOINT_FULL}/token/create`;
  // 'http://localhost:57350/api/v0/token/create';

  constructor(private http: HttpClient) { }

  getToken(creditCard: string, StoreId: number, email: string) {


    const  newdata = {
      'StoreId': StoreId,
      'NoCreditCard': creditCard,
      'Action': '1',
      'Email': email
    };


    const headers = new HttpHeaders()
    .append('Content-Type', 'application/json');

    return  this.http.post( this.tokenUrl, JSON.stringify(newdata), {headers: headers})
    .map( response => response
    );
  }
}
