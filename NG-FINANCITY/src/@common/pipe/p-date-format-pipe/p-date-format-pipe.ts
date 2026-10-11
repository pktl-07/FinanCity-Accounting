import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { SubSink } from 'src/@common/service/utility-service';

@Pipe({
    name: 'pDateformat',
})
export class PDateFormatPipe implements PipeTransform, OnDestroy {

    _intl: IntlService;
    public _subsink = new SubSink()

    constructor(intl: IntlService) {

        this._intl = intl;
        this._subsink.sink = this._intl.changes.subscribe();
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    transform(value: any, ...args: any[]): any {

        if (value === undefined || value === null) {
            return null;
        }
        else if (typeof (value) === 'number') {
            value = value.toString();
        }
        try {
            return `${value.substring(6, 8)}-${value.substring(4, 6)}-${value.substring(0, 4)}`;
        } catch {
            return null;
        }
    }
}