import { Pipe } from '@angular/core';
import { commissoinType } from 'src/@common/service/utility-service';

@Pipe({
    name: 'pCommissionStructurePipe',
})
export class PCommissionStructurePipe {

    transform(value: any, ...args: any[]): any {

        const t = commissoinType().filter(x => x.value == value);
        if (t.length > 0)
            return commissoinType().filter(x => x.value == value)[0].text;
        else
            return 'None';
    }
}