import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'sum'
})
export class SumPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let total = 0;
    for(let r of value) {
      total += parseFloat(r[args[0]]);
    }
    return total;
  }
}
