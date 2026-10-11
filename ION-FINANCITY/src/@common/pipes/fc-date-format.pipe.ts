import { Injectable, Pipe, PipeTransform } from '@angular/core';

@Injectable({ providedIn: 'root' })
@Pipe({
    name: 'fcDateFormat'
})
export class FCDateFormatPipe implements PipeTransform {
    transform(value: any, ...args: any[]): string {

        if (value === undefined || value === null)
            return null;

        else if (typeof (value) === 'number')
            value = value.toString();


        if (args.includes('yyyymmdd'))
            return value.substring(0, 4) + "/" + value.substring(4, 6) + "/" + value.substring(6, 8);
        else
            return value.substring(6, 8) + "/" + value.substring(4, 6) + "/" + value.substring(0, 4);

    }
}