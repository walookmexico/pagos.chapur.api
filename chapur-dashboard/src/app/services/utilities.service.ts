import { Injectable } from '@angular/core';
import { DatePipe } from '@angular/common';

@Injectable()
export class UtilitiesService {

  constructor() { }

  // Realiza la conversión de formato de una fecha dada MM/dd/yyyy
  dateFormatYYYYMMDD(date): string {
    const date2 =  new DatePipe('en-US');
    return date2.transform(date.toString(), 'yyyy-MM-dd');
  }

  // Realiza la conversión de formato de una fecha dada dd-MM-yyyy
  dateFormatDDMMYYYY(date): string {
    const date2 =  new DatePipe('en-US');
    return date2.transform(date.toString(), 'dd-MM-yyyy');
  }

  // Crea un nuevo array agrupado por la propiedad enviada
  groupBy(list, keyGetter): any {
    const map = new Map();
    list.forEach(element => {
      const key = keyGetter(element);
      const collection = map.get(key);

      if (!collection) {
        map.set(key,[element]);
      } else {
        collection.push(element);
      }

    });

    return map;
  }

}
