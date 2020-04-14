import { Injectable } from '@angular/core';
import { Appsettings } from '../configuration/appsettings';
import { HttpService } from './http.service';
import { HttpParams } from '@angular/common/http';
import { URLSearchParams } from '@angular/http';
import { UtilitiesService } from '../services/utilities.service';

@Injectable()
export class CatalogsService {

  constructor( private httpService: HttpService ) { }

  getCatalogsDashboard() {
    let apiUrl  = `${Appsettings.API_ENDPOINT_FULL}/catalogs/dashboard`;
    return this.httpService.get(apiUrl).map(res => res.json());
  }

}
