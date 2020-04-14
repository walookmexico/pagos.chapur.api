import { Injectable } from '@angular/core';
import { TokenService } from './token.service';
@Injectable()
export class ModalsService {

  constructor(private token: TokenService) { }

  modalToken(phone: string) {
    const options = {
          header: 'El token de confirmación ha sido enviado al celular con terminación:  ' + phone,
          color: '#a94442', // red, blue....
          widthProsentage: 40, // The with of the popou measured by browser width
          animationDuration: 1, // in seconds, 0 = no animation
          showButtons: true, // You can hide this in case you want to use custom buttons
          confirmBtnContent: 'Confirmar', // The text on your confirm button
          cancleBtnContent: 'Cancelar', // the text on your cancel button
          confirmBtnClass: 'btn btn-success', // your class for styling the confirm button
          cancleBtnClass: 'btn btn-danger', // you class for styling the cancel button
          animation: 'fadeInDown' // 'fadeInLeft', 'fadeInRight', 'fadeInUp', 'bounceIn','bounceInDown'
    };
    return options;
  }

  modalPagoExitoso() {

    const options = {
      showButtons: false,
      color: '#42A948',
      header: 'Notificación...'
    };

    return options;
  }

  modalPagoErroneo() {
    const options = {
      showButtons: true,
      color: '#a94442',
      header: 'Notificación...',
      confirmBtnContent: 'Aceptar',
      cancleBtnContent: 'Cancelar',
      confirmBtnClass: 'btn btn-success',
      cancleBtnClass: 'btn btn-danger',
      animation: 'fadeInDown',
      data: {'msg': 'error'}
    };
    return options;
  }

  getToken(creditCart: string, idStore: number, email: string) {
    this.token.getToken(creditCart, idStore, email).subscribe(respose => console.log(respose));

  }

}
