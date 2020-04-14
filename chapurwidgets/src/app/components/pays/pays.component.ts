import { Component, OnInit, ViewChild, Input} from '@angular/core';

import { NgForm, FormGroup,  FormControl, Validators } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';

// Servicios
import { TokenService } from '../../services/token.service';
import { PayService } from '../../services/pay.service';
import { ModalsService } from '../../services/modals.service';

// Modelos
import { PaymentDetail } from '../../models/paymentDetail.model';
import { Product } from '../../models/product.model';
import { ProductDetail } from '../../models/productDetail.model';

// Componente de modal
import { MatDialog } from '@angular/material';
import { DialogsTokenComponent } from '../dialogs-token/dialogs-token.component';
import { DialogsAlertsComponent } from '../dialogs-alerts/dialogs-alerts.component';
import { DialogsAlertsSuccesComponent } from '../dialogs-alerts-succes/dialogs-alerts-succes.component';

@Component({
  selector: 'app-pays',
  templateUrl: './pays.component.html',
  styleUrls: ['./pays.component.css']
})
export class PaysComponent implements OnInit {

  payDetail: PaymentDetail;
  phone: string;
  data: any = [];
  errors: string[];
  errorsdetails: string[];
  errorsdetailsParameters: string[];
  articulo: string[];
  isValidateParameters: boolean;
  isvalidateDetail: boolean;
  isvalidateDetailParameters: boolean;
  working: boolean;
  message: string;
  showError: boolean;

  frmTarget: FormGroup;

  constructor(
    private route: ActivatedRoute,
  private tokenService: TokenService,
    private payService: PayService,
    private modal: ModalsService,
    public dialog: MatDialog
  ) {
    this.errors = [];
    this.articulo = [];
    this.errorsdetails = [];
    this.errorsdetailsParameters = [];
    this.message = '';

    this.frmTarget = new FormGroup({
      'name': new FormControl('', [Validators.required]),
      'numberTarget': new FormControl('', [ Validators.required,
                                            Validators.maxLength(16),
                                            Validators.minLength(16)]),
      'month': new FormControl('', [ Validators.required,
        Validators.maxLength(2),
        Validators.minLength(2)]),
      'year': new FormControl('', [ Validators.required,
          Validators.maxLength(2),
          Validators.minLength(2)]),
      'cvv': new FormControl('', [ Validators.required,
            Validators.maxLength(3),
            Validators.minLength(3)]),
    });
  }

  ngOnInit() {
    this.working = true;
    this.route.queryParams.subscribe(params => {

      // let creditCardDate: string;
      this.payDetail = new PaymentDetail();
      // creditCardDate = '0810';

      this.payDetail.url              = params['url'];
      this.payDetail.amount           = params['total'];
      this.payDetail.idStore          = params['idTienda'];
      this.payDetail.email            = params['correo'];
      this.payDetail.idPurchaseOrder  = params['ordenCompra'];
      this.payDetail.platformId       = params['plataformaId'];
      this.payDetail.userPlatform     = params['usuarioPlataforma'];
      this.payDetail.passwordPlatform = params['password'];
      this.articulo                   = params['articulo'];
      this.payDetail.months           = params['meses'];
      this.payDetail.withInterest     = params['conIntereses'];

      // Validamos los parametros generales
      this.validateParameters();
      // Validamos los parametros del detalle
      if (!this.isValidateParameters) {
        this.validateDetail(params['articulo']);
      }
      // Validamos los parametros requeridos del detalle
      if (!this.isValidateParameters && !this.isvalidateDetail) {
        this.validateDetailProduct(params['articulo']);
      }

      if (!this.isValidateParameters && !this.isvalidateDetail && !this.isvalidateDetailParameters) {
          this.payDetail.lstProduct       = this.getArticulos(params['articulo']);
          this.payDetail.detail           = this.getDetailProdcut(params['articulo']);
      }
    });
    this.working = false;
  }

  getDetailProdcut(detail: any): ProductDetail[] {
    let detailProducts: ProductDetail[] = [new ProductDetail()];
    let productDetail: string[] = [];
    let product: ProductDetail;

    detailProducts = [];
    if (detail.length > 20) {
      productDetail = detail.split(',');
        product       = new ProductDetail();

        product.itemId            = Number(productDetail[7].toString());
        product.priceWithoutTax   = Number(productDetail[8].toString());
        product.iva               = Number(productDetail[9].toString());
        product.ieps              = Number(productDetail[10].toString());
        product.averageCost       = Number(productDetail[11].toString());
        product.quantity          = Number(productDetail[4].toString());
        product.discount          = Number(productDetail[12].toString());

        detailProducts.push(product);
    } else {
      for (const p of detail) {

        productDetail = p.split(',');
        product       = new ProductDetail();

        product.itemId            = Number(productDetail[7].toString());
        product.priceWithoutTax   = Number(productDetail[8].toString());
        product.iva               = Number(productDetail[9].toString());
        product.ieps              = Number(productDetail[10].toString());
        product.averageCost       = Number(productDetail[11].toString());
        product.quantity          = Number(productDetail[4].toString());
        product.discount          = Number(productDetail[12].toString());

        detailProducts.push(product);
      }
    }

    return detailProducts;
  }

  getArticulos(detail: any): Product[] {
    let products: Product[]     = [new Product()];
    let productDetail: string[] = [];
    let product: Product;

    products = [];
    if (detail.length > 20) {
      productDetail = detail.split(',');
      product       = new Product();

      product.description = productDetail[0];
      product.model       = productDetail[1];
      product.color       = productDetail[2];
      product.size        = productDetail[3];
      product.quantity    = Number(productDetail[4].toString());
      product.price       = Number(productDetail[5].toString());
      product.total       = Number(productDetail[6].toString());

      products.push(product);
    } else {
      for (const p of detail) {

          productDetail = p.split(',');
          product       = new Product();

          product.description = productDetail[0];
          product.model       = productDetail[1];
          product.color       = productDetail[2];
          product.size        = productDetail[3];
          product.quantity    = Number(productDetail[4].toString());
          product.price       = Number(productDetail[5].toString());
          product.total       = Number(productDetail[6].toString());

          products.push(product);
      }
    }

    return products;
  }

  onSubmit(): void {
    this.working = true;

    this.payDetail.noCreditCard   = this.frmTarget.value.numberTarget;
    this.payDetail.nameCreditCard = this.frmTarget.value.name;
    this.payDetail.CVV            = this.frmTarget.value.cvv;
    this.payDetail.createDate     = `${this.frmTarget.value.month}${this.frmTarget.value.year}`;

    this.tokenService.getToken( this.payDetail.noCreditCard,
                                this.payDetail.idStore,
                                this.payDetail.email).subscribe(response => {
                                      if (response['isSuccess']) {
                                        this.showError = false;
                                        this.data = response['data'];
                                        this.phone = `****** ${this.data['telefono'].substring(6, 10)}`;
                                        // Mostramos el modal para capturar el token
                                        setTimeout(() => {
                                          this.working = false;
                                          this.modalToken();
                                        }, 2000);
                                      } else {
                                        this.working = false;
                                        this.showError = true;
                                        if ( response['data'] ) {
                                          let data     = response['data'];
                                          this.message = data['mensaje'];
                                          if ( data['estatus'] === 2 ) {
                                            this.message = 'Número de tarjeta de crédito incorrecto';
                                          }
                                        } else {
                                          this.message = response['messages'];
                                        }
                                      }
                              });

  }


  onClickConfirmToken() {
    this.showError = false;
    this.working = true;

    this.payService.payDetail(this.payDetail).subscribe(response => {
      if (response['isSuccess']) {
        // this.mdlConfirmToken.hide();
        this.working = false;

        const dialogRef = this.dialog.open(DialogsAlertsSuccesComponent, {
          width: '500px',
          disableClose : true,
          data: {'title': 'Error al realizar el pago', 'message': response['messages']}
        });

        setTimeout(() => {
          window.location.href = `https://${this.payDetail.url}`;
        }, 3000);

      } else {
        this.working = false;
        const data = response['data'];
        let message = '';

        if ( data ) {
           message = `${data.mensaje}`;
        } else {
          message = response['messages']
        }

        const dialogRef = this.dialog.open(DialogsAlertsComponent, {
          width: '500px',
          disableClose : true,
          data: {'title': 'Error al realizar el pago', 'message': message }
        });
      }
    },
    err => {
      const er = err['error'];

      const dialogRef = this.dialog.open(DialogsAlertsComponent, {
        width: '500px',
        disableClose : true,
        data: {'title': 'Notificación', 'message': er['message'] }
      });

      this.working = false;
    });
  }

  onClickCancelToken() {
    // this.mdlConfirmToken.hide();
  }

  modalToken() {
    const dialogRef = this.dialog.open(DialogsTokenComponent, {
      width: '450px',
      disableClose : true,
      data: {
        'phone': this.phone,
        'numberTarget': this.payDetail.noCreditCard,
        'idStore': this.payDetail.idStore,
        'email': this.payDetail.email
      }
    });

    // obtenemos la respuesta del modal
    dialogRef.afterClosed().subscribe(response => {
      if (response['status']) {
        this.payDetail.token = response['token'].token;
        this.onClickConfirmToken();
      } else {
        console.log('Token cancelado');
      }
    });
  }

  validateParameters (): void {
    if (this.payDetail.url == null) {
      this.errors.push('La url para direccionar despues del pago es requerida');
    }

    if (this.payDetail.amount == null) {
      this.errors.push('El monto total de la compra es requerido');
    }

    if (this.payDetail.idStore == null) {
      this.errors.push('El identificador de la compra es requerido');
    }

    if (this.payDetail.email == null) {
      this.errors.push('El correo electrónico del cliente es requerido');
    }

    if (this.payDetail.idPurchaseOrder == null) {
      this.errors.push('El número de orden de compra es requerido');
    }

    if (this.payDetail.platformId == null) {
      this.errors.push('El identificador de la plataforma es requerido');
    }
    if (this.payDetail.userPlatform == null) {
      this.errors.push('El usuario de la plataforma es requerido');
    }
    if (this.payDetail.passwordPlatform == null) {
      this.errors.push('La contraseña de la plataforma es requerido');
    }
    if (this.articulo == null) {
      this.errors.push('El listado de productos comprados es requerido');
    }

    if ( this.payDetail.months == null ) {
      this.errors.push('El parámetro de meses es requerido');
    }

    if ( this.payDetail.withInterest == null ) {
      this.errors.push('El parámetro con intereses es requerido');
    }

    if ( this.errors.length > 0 ) {
      this.isValidateParameters = true;
    }
  }

  validateDetail(detail: any): void {
    let productDetailCount: string[] = [];

    let count = 0;
    if (detail.length > 20) {
      productDetailCount = detail.split(',');
        if (productDetailCount.length !== 13) {
          // tslint:disable-next-line:max-line-length
          this.errorsdetails.push(`El detalle del producto no esta completo debe contener 13 elementos, en caso de valor nullo o vacío poner la coma del espacio`);
      }
    } else {
      for (const p of detail) {
        count ++;
        productDetailCount = p.split(',');

        if (productDetailCount.length !== 13) {
          // tslint:disable-next-line:max-line-length
          this.errorsdetails.push(`El detalle del producto No. ${count} de la lista no esta completo debe contener 13 elementos, en caso de valor nullo o vacío poner la coma del espacio`);
        }
      }
    }

    if ( this.errorsdetails.length > 0 ) {
      this.isvalidateDetail = true;
    }
  }

  validateDetailProduct(detail: any): void {
    let productDetailCount: string[] = [];
    let count = 0;
    this.errorsdetailsParameters = [];
    for (const p of detail) {
      count ++;
      productDetailCount = p.split(',');

      if (productDetailCount[4] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`La cantidad del producto No. ${count} de la lista es requerida `);
      }
      if (productDetailCount[5] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El precio del producto No. ${count} de la lista es requerido `);
      }
      if (productDetailCount[6] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El total del producto No. ${count} de la lista es requerido `);
      }
      if (productDetailCount[7] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El identificador del producto No. ${count} de la lista es requerido `);
      }
      if (productDetailCount[8] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El precio sin iva del producto No. ${count} de la lista es requerido `);
      }
      if (productDetailCount[9] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El IVA del producto No. ${count} de la lista es requerido `);
      }
      if (productDetailCount[10] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El IEPS del producto No. ${count} de la lista es requerido `);
      }
      if (productDetailCount[11] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El costo promedio del producto No. ${count} de la lista es requerido `);
      }
      if (productDetailCount[12] === '') {
        // tslint:disable-next-line:max-line-length
        this.errorsdetailsParameters.push(`El descuento del producto No. ${count} de la lista es requerido `);
      }

    }

    if ( this.errorsdetailsParameters.length > 0 ) {
      this.isvalidateDetailParameters = true;
    }
  }
}
