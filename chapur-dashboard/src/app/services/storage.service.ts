import { Injectable } from '@angular/core';
import { PersistenceService, StorageType } from 'angular-persistence';
import { Router, ActivatedRoute } from '@angular/router';

@Injectable()
export class StorageService {

  constructor(private persistenceService: PersistenceService,
     private router: Router) { }

     get token (): string {
       return this.persistenceService.get( 'token', StorageType.LOCAL );
     }

     set token ( token: string ) {
       this.persistenceService.remove('token');
       this.persistenceService.set('token', token, { type: StorageType.LOCAL, timeout: 3600000 });
     }

     get name(): string {
       return this.persistenceService.get( 'name', StorageType.LOCAL );
     }

     set name (name: string) {
      this.persistenceService.remove('name');
      this.persistenceService.set('name', name, { type: StorageType.LOCAL });
     }

     get userName (): string {
       return this.persistenceService.get( 'username', StorageType.LOCAL );
     }

     set userName( username: string ) {
       this.persistenceService.remove('username');
       this.persistenceService.set( 'username', username,  { type: StorageType.LOCAL } );
     }

     get email (): string {
       return this.persistenceService.get( 'email', StorageType.LOCAL );
     }

     set email (email: string) {
       this.persistenceService.remove('email');
       this.persistenceService.set( 'email', email, { type: StorageType.LOCAL} );
     }

     get rol(): string {
       return this.persistenceService.get( 'rol', StorageType.LOCAL );
     }

     set rol (rolId: string) {
       this.persistenceService.remove('rol');
       this.persistenceService.set( 'rol', rolId, { type: StorageType.LOCAL } );
     }

    get changePassword (): boolean {
      return this.persistenceService.get('changePassword', StorageType.LOCAL);
    }

    set changePassword (changePassword: boolean) {
      this.persistenceService.remove('changePassword');
      this.persistenceService.set( 'changePassword', changePassword, { type: StorageType.LOCAL} );
    }

     loggedIn (): boolean {
      if (this.token === undefined || this.token === '') {
        return false;
      } else {
        return true;
      }
     }

     logout(): void {
        this.persistenceService.removeAll(StorageType.LOCAL);
     }

     logoutRedirect(): void {
      this.logout();
      this.router.navigateByUrl('login');
     }

}
