import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TokenService } from '../../services/token.service';
import { PayService } from '../../services/pay.service';
import { Pay } from '../../models/pay.model';

@Component({
  selector: 'app-paycreditcard',
  templateUrl: './paycreditcard.component.html',
  styleUrls: ['./paycreditcard.component.css']
})
export class PaycreditcardComponent implements OnInit {

  @ViewChild('formTarjeta') formTarjeta: NgForm;
  @ViewChild('formToken') formToken: NgForm;

  pay: Pay;
  creditNumberPart1: string;
  creditNumberPart2: string;
  creditNumberPart3: string;
  creditNumberPart4: string;
  name: string;
  cvv: number;
  showFront: boolean;
  month: string;
  year: string;
  showToken: boolean;
  showError: boolean;
  message: string;
  token: number;
  showForm: boolean;
  phone: string;
  data: any = [];
  years: number[];
  errors: string[];
  isValidateParameters: boolean;
  working: boolean;
  authorizationCode: string;
  typeTargetClass = 'credit-card-disabled';

  constructor(
    private route: ActivatedRoute,
    private payService: PayService,
    private tokenService: TokenService
  ) {

    this.pay = null;
    this.creditNumberPart1 = null;
    this.creditNumberPart2 = null;
    this.creditNumberPart3 = null;
    this.creditNumberPart4 = null;

    this.name = '';
    this.cvv = null;

    this.showFront = false;
    this.month = this.year = '00';
    this.showToken = false;
    this.showError = false;
    this.showForm = false;
    this.phone = '';
    this.data = [];
    this.years = [];
    this.errors = [];

    this.isValidateParameters = false;
    this.working = false;

    this.message = '';


    let year = new Date();
    for (let i = 0; i < 16; i++) {
      this.years.push(2009 + i);
      // this.years.push(year.getFullYear() + i);
    }

    /*
    this.creditNumberPart1 = '6484';
    this.creditNumberPart2 = '9931';
    this.creditNumberPart3 = '0057';
    this.creditNumberPart4 = '8580';

    this.month = '08';
    this.year = '10';

    this.name = 'PACHECO AVILES ROMEL D';
    this.cvv = 902;
    */



  }

  ngOnInit() {
    this.working = true;
    this.route.queryParams.subscribe(params => {

      this.pay = new Pay();

      this.pay.amount             = params['total'];
      this.pay.idStore            = params['idTienda'];
      this.pay.email              = params['correo'];
      this.pay.idPurchaseOrder    = params['ordenCompra'];
      this.pay.platformId         = params['plataformaId'];
      this.pay.userPlatform       = params['usuarioPlataforma'];
      this.pay.passwordPlatform   = params['password'];
      this.pay.months             = params['meses'];
      this.pay.withInterest       = params['conIntereses'];

      this.validateParameters();
      this.working = false;

    });
  }

  onSubmit(event: any): void {
    event.preventDefault();

    this.pay.noCreditCard   = this.creditNumberPart1 + this.creditNumberPart2 + this.creditNumberPart3 + this.creditNumberPart4;
    this.pay.nameCreditCard = this.name;
    this.pay.CVV            = this.cvv;
    this.pay.createDate     = this.month + this.year;

    this.working = true;
    this.showError = false;

    this.tokenService.getToken(this.pay.noCreditCard,
      this.pay.idStore,
      this.pay.email).subscribe(res => {

        if (res['isSuccess']) {
          this.data = res['data'];
          this.phone = `******${this.data['telefono'].substring(6, 10)}`;
          this.showToken = true;
          this.showError = false;
        } else {
          this.showError = true;
          if ( res['data'] ) {
            let data     = res['data'];
            this.message = data['mensaje'];

            if ( data['estatus'] === 2 ) {
              this.message = 'Número de tarjeta de crédito incorrecto';
            }
          } else {
            this.message = res['messages'];
          }
        }

        this.working = false;

      }, err => {
        this.validateResponseError(err);
      });
  }

  onPago(event: any): void {

    event.preventDefault();

    this.working   = true;
    this.showError = false;
    this.pay.token = this.token;

    this.payService.payCreditcard(this.pay).subscribe(res => {
      if (res['isSuccess']) {
        this.showForm  = true;
        this.showError = false;

        let data = res['data'];
        this.authorizationCode = data['idAutorizacion'];

      } else {

        if ( res['data'] ) {
          let data    = res['data'];
          this.message = data['mensaje'];

          if (data['estatus'] === -1 || data['estatus'] === -7 || data['estatus'] === -2) {
            this.showToken = false;
          }

          this.token = null;
        } else {
          this.message = res['messages'];
        }

        this.showForm  = false;
        this.showError = true;
      }

      this.working = false;
    }, err => {
      this.validateResponseError(err);
    });
  }

  getToken(): void {

    this.working = true;
    this.showError = false;

    this.tokenService.getToken(this.pay.noCreditCard,
      this.pay.idStore,
      this.pay.email).subscribe(res => {

        this.data = res['data'];

        if (res['isSuccess']) {
          this.phone     = `******${this.data['telefono'].substring(6, 10)}`;
          this.showToken = true;
          this.showError = false;
        } else {
          this.showError = true;
          if ( res['data'] ) {
            this.message = this.data['mensaje'];
            if ( this.data['estatus'] === 2 ) {
              this.message = 'Número de tarjeta de crédito incorrecto';
            }
          }
        }

        this.working = false;

      }, err => {
        this.validateResponseError(err);
      });
  }

  validateParameters (): void {
    if ( this.pay.amount == null ) {
      this.errors.push('El monto total de la compra es requerido');
    }

    if ( this.pay.idStore == null ) {
      this.errors.push('El identificador de la compra es requerido');
    }

    if ( this.pay.email == null ) {
      this.errors.push('El correo electrónico del cliente es requerido');
    }

    if ( this.pay.idPurchaseOrder == null ) {
      this.errors.push('El número de orden de compra es requerido');
    }

    if ( this.pay.platformId == null ) {
      this.errors.push('El identificador de la plataforma es requerido');
    }

    if ( this.pay.userPlatform == null ) {
      this.errors.push('El usuario de la plataforma es requerido');
    }

    if ( this.pay.passwordPlatform == null ) {
      this.errors.push('La contraseña de la plataforma es requerido');
    }

    if ( this.pay.months == null ) {
      this.errors.push('El parámetro de meses es requerido');
    }


    if ( this.pay.withInterest == null ) {
      this.errors.push('El parámetro conIntereses es requerido');
    }

    if ( this.errors.length > 0 ) {
      this.isValidateParameters = true;
    }
  }

  validateResponseError(err: any): void {

        this.showError = true;
        if ( err.status === 0) {
          this.message = 'Problemas de conexión con el servicor, verifique su conexión a internet';
        }

        if ( err.status === 404) {
          this.message = 'Problemas al solicitar el token al servidor';
        }

        if ( err.status === 500) {
          this.message = 'Problemas al ejecutar la petición en el servidor';
        }

        this.working = false;
  }

  validateTypeTarget(): void {
    if (this.creditNumberPart2.length <= 2) {
      let number = this.creditNumberPart1 + this.creditNumberPart2;
      console.log(number);
      switch (number) {
        case '648410': {
          this.typeTargetClass = 'credit-card-cree';
           break;
        }
        case '648420': {
          this.typeTargetClass = 'credit-card-clasica';
           break;
        }
        case '648430': {
          this.typeTargetClass = 'credit-card-platinum';
          break;
       }
        default: {
          this.typeTargetClass = 'credit-card-disabled';
           break;
        }
     }
    }
  }
}
