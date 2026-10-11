import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'kendoDateformat',
})
export class KendoDateFormatPipe implements PipeTransform {
  transform(value: any, ...args: any[]): any {

    if (value === undefined || value === null || value == '')
      return null;
    else if (typeof (value) === 'number')
      value = value.toString();

    const d = new Date(`${value.substring(0, 4)}-${value.substring(4, 6)}-${value.substring(6, 8)}`);
   
    if (!isNaN(d.getTime())) 
      return d;
    
    return null;
  }
}