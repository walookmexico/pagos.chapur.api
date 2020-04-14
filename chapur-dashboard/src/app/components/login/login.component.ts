import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';
import { UsersService   } from '../../services/users.service';
import { StorageService } from '../../services/storage.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Appsettings } from '../../configuration/appsettings';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  frmSignin: FormGroup;
  working: boolean;
  loginFailed: boolean;

  alertMessages: string;

  user: Object = {
    username: '',
    password: ''
  };

  constructor( private userService: UsersService,
               private storageService: StorageService,
               private router: Router) {


    this.storageService.logout();

    this.frmSignin = new FormGroup({
      'username': new FormControl('', [ Validators.required,
                                        Validators.maxLength(30) ]),
      'password': new FormControl('', [ Validators.required ])
    });

    this.frmSignin.setValue(this.user);

  }

  ngOnInit() {
    console.log(Appsettings.API_ENDPOINT);
    console.log(Appsettings.ESLACTIC);
    console.log(Appsettings.API_ENDPOINT_FULL);
  }

  login() {

    this.working     =  true;
    this.loginFailed = false;

     this.userService.login(this.frmSignin.value.username , this.frmSignin.value.password)
    .subscribe(res => {

      if ( res.access_token ) {

        this.storageService.token          = res.access_token ;
        this.storageService.email          = res.email;
        this.storageService.name           = res.fullName;
        this.storageService.userName       = this.frmSignin.value.username;
        this.storageService.rol            = res.rol;
        this.storageService.changePassword = res.changePassword === 'False' ? false : true;

        this.router.navigateByUrl('dashboard');
      } else {
        this.loginFailed = true;
      }
      this.working = false;
    }, err => {
      if (err.status === 0) {
        this.alertMessages = 'Problemas de conexión con el servidor';
      } else {
        this.alertMessages = 'Usuario y/o contraseña equivocado';
       }
      this.working = false;
      this.loginFailed = true;
    });
  }

}
