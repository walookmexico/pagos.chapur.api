import { Product } from './product.model';
import { ProductDetail } from './productDetail.model';
export class PaymentDetail {

  url: string;
  amount: number;
  idStore: number;
  email: string;
  createDate: string;
  idPurchaseOrder: number;
  platformId: number;
  userPlatform: string;
  passwordPlatform: string;
  token: string;
  noCreditCard: string;
  nameCreditCard: string;
  CVV: number;
  lstProduct: Product[];
  detail: ProductDetail[];
  months: number;
  withInterest: number;

  constructor() {
    this.lstProduct = [new Product()];
    this.detail = [new ProductDetail()];
  }
}
