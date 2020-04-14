import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivate } from '@angular/router';
import { StorageService } from './storage.service';


@Injectable()
export class AuthGuardService implements CanActivate {

  constructor(private stotageService: StorageService,
              private route: Router ) { }

  canActivate( next: ActivatedRouteSnapshot, state: RouterStateSnapshot ) {

    if ( this.stotageService.loggedIn() ) {

      if (state.url === '/users' && this.stotageService.rol === 'Consulting') {
        this.route.navigateByUrl('login');
      }

      return true;

    } else {
      this.route.navigateByUrl('login');
    }
  }

}
