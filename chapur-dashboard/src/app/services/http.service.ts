import { Injectable } from '@angular/core';
import { Http, XHRBackend, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { StorageService } from './storage.service';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { extend } from 'webdriver-js-extender';

@Injectable()

export class HttpService  extends Http {

  constructor( backend: XHRBackend,
               options: RequestOptions,
               private storageService: StorageService ) {
   super(backend, options);
  }

  request(url: string|Request, options?: RequestOptionsArgs): Observable<Response> {
    const TOKEN = this.storageService.token;
    if (typeof url === 'string') {
      if (!options) {
        options = {headers: new Headers()};
      }
      options.headers.set('Authorization', `Bearer ${TOKEN}`);
    } else {
      url.headers.set('Authorization', `Bearer ${TOKEN}`);
    }

    return super.request(url, options).catch(this.catchAuthError(this));
  }

  private catchAuthError (self: HttpService) {
    return (res: Response) => {
      if (res.status === 401 || res.status === 403) {
        this.storageService.logoutRedirect();
      }
      return Observable.throw(res);
    };
  }

}
