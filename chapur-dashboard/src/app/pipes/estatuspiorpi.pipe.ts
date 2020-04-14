import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'estatuspiorpi'
})
export class EstatuspiorpiPipe implements PipeTransform {

  transform(value: any, args?: any): any {

     let status = '';

      switch ( value ) {
       case 1:
       status  = 'CORRECTO';
         break;
       case 2:
       status  = 'PENDIENTE';
         break;
       case 3:
       status  = 'ALERTA';
         break;
      }

    return status;
  }

}
