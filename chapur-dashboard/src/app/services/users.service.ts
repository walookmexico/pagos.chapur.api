import { Injectable } from '@angular/core';
import { StorageService } from './storage.service';
import { HttpService } from './http.service';
import { Http, Headers, URLSearchParams } from '@angular/http';
import { Appsettings } from '../configuration/appsettings';
import 'rxjs/add/operator/map';

@Injectable()
export class UsersService {
  apiUrlUserFull = `${Appsettings.API_ENDPOINT_FULL}/user`;
  constructor(
    private storageServie: StorageService,
    private http: Http,
    private httpService: HttpService
  ) {}

  login(userName: string, password: string) {
    let apiUrl = `${Appsettings.API_ENDPOINT}/token`;
    let headers = new Headers();
    let body = `username=${userName}&password=${password}&grant_type=password`;
    headers.append('Content-Type', 'application/x-www-form-urlencoded');

    return this.http
      .post(apiUrl, body, { headers: headers })
      .map(res => res.json());
  }

  getAllUser() {
    return this.httpService.get(this.apiUrlUserFull).map(res => res.json());
  }

  postUser(user: any) {
    let headers = new Headers();
    headers.append('Content-Type', 'application/json');
    return this.httpService
      .post(this.apiUrlUserFull, JSON.stringify(user), { headers: headers })
      .map(res => res.json());
  }

  deleteUser(idUser: number) {
    let params = new URLSearchParams();
    params.append('userId', idUser.toString());
    return this.httpService
      .delete(this.apiUrlUserFull, { search: params } )
      .map(res => res.json());
  }

  getUserUserId(idUser: any) {
    let params = new URLSearchParams();
    params.append('userId', idUser);
    return this.httpService
      .get(this.apiUrlUserFull, { search: params })
      .map(res => res.json());
  }

  updateUser(user: any) {
    let headers = new Headers();
    headers.append('Content-Type', 'application/json');
    return this.httpService
      .put(this.apiUrlUserFull, JSON.stringify(user), { headers: headers })
      .map(res => res.json());
  }

  forgotPassword(email: string) {
    let headers = new Headers();
    let url = `${Appsettings.API_ENDPOINT_FULL}/user/RecoveryPassword`;
    headers.append('Content-Type', 'application/json');
     let params = {
       'email': email
     };

    return this.httpService
      .post(url, JSON.stringify(params), { headers: headers })
      .map(res => res.json());
  }

  updatePassword(lastPassword: string, newPassword: string) {
    let headers = new Headers();
    let url = `${Appsettings.API_ENDPOINT_FULL}/user/UpdatePassword`;
    headers.append('Content-Type', 'application/json');
     let params = {
       'LastPassword': lastPassword,
       'NewPassword': newPassword
     };

     return this.httpService
      .post(url, JSON.stringify(params), { headers: headers })
      .map(res => res.json());
  }
}
