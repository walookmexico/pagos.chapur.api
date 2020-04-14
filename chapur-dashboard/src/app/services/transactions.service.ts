import { Injectable } from '@angular/core';
import { Appsettings } from '../configuration/appsettings';
import { HttpService } from './http.service';
import { HttpParams } from '@angular/common/http';
import { URLSearchParams } from '@angular/http';
import { UtilitiesService } from '../services/utilities.service';


@Injectable()
export class TransactionsService {

  constructor(private httpService: HttpService ,
              private utilitiesService: UtilitiesService ) { }

    setHeaderContentTypeToJson(headers: Headers) {
      headers.append('Content-Type', 'application/json');
    }

    filterTransaction( startDate: Date, endDate: Date, storeId: any[], platformId: any[] ) {

      let apiUrl  = `${Appsettings.API_ENDPOINT_FULL}/transaction/filter`;
      let headers = new Headers();
      let parameters  = new URLSearchParams();

       if (storeId.length === 0) {
        storeId.push(0);
       }

       if (platformId.length === 0) {
        platformId.push(0);
       }

      parameters.append('startDate', this.utilitiesService.dateFormatYYYYMMDD(startDate)) ;
      parameters.append('endDate',  this.utilitiesService.dateFormatYYYYMMDD(endDate));
      parameters.append('storeId', storeId.toString());
      parameters.append('platformId', platformId.toString());

      return this.httpService.get(apiUrl, { search: parameters }).map(res => res.json());

    }

    filterStatus( startDate: Date, endDate: Date,  storeId: any[], platformId: any[]) {

      let apiUrl  = `${Appsettings.API_ENDPOINT_FULL}/transaction/status`;
      let headers = new Headers();
      let parameters  = new URLSearchParams();

      if (storeId.length === 0) {
        storeId.push(0);
       }

       if (platformId.length === 0) {
        platformId.push(0);
       }


      parameters.append('startDate', this.utilitiesService.dateFormatYYYYMMDD(startDate)) ;
      parameters.append('endDate',  this.utilitiesService.dateFormatYYYYMMDD(endDate));

      parameters.append('storeId', storeId.toString());
      parameters.append('platformId', platformId.toString());
      return this.httpService.get(apiUrl, { search: parameters }).map(res => res.json());

    }

    syncStatusByDate(startDate: Date, endDate: Date) {
      const API_URL = `${Appsettings.API_ENDPOINT_FULL}/transaction/checkstatusbydate`;
      let parameters: URLSearchParams;

      parameters = new URLSearchParams();
      parameters.append('startDate', this.utilitiesService.dateFormatYYYYMMDD(startDate)) ;
      parameters.append('endDate',  this.utilitiesService.dateFormatYYYYMMDD(endDate));

      return this.httpService.get(API_URL,  { search: parameters}).map(res => res.json());
    }
}
