import { Injectable, Pipe, PipeTransform } from '@angular/core';

@Injectable({ providedIn: 'root' })
@Pipe({
    name: 'fcTimeFormat'
})
export class FCTimeFormatPipe implements PipeTransform {
    transform(value: any, ...args: any[]): any {

        if (value === undefined || value === null)
            return null;

        let a = 'AM';
        let h = +value.substr(0, 2);
        let m = value.substr(2, 2);

        if (h >= 0 && h < 12) {
            a = 'AM';
        }
        else if (h == 12) {
            a = 'PM';
        }
        else if (h > 12) {
            h = h - 12;
            a = 'PM';
        }

        return `${h} : ${m} ${a}`;
    }
}