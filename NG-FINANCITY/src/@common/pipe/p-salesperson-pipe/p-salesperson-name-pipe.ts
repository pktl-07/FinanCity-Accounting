import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { Subject, from } from 'rxjs';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { takeUntil } from 'rxjs/operators';
import { kendoFilter } from 'src/@common/service/utility-service';

@Pipe({
    name: 'pSalespersonName',
})
export class PSalespersonNamePipe implements PipeTransform, OnDestroy {

    public _unsubscribe = new Subject<any>();

    constructor(private _salespersonManagementService: SalespersonManagementService) {
    }

    ngOnDestroy(): void {

        this._unsubscribe.next();
        this._unsubscribe.complete();
    }

    transform(value: any, ...args: any[]): any {

        if (value === undefined || value === null)

            return null;
        else if (value === '1') {

            return new Promise((resolve) => {
                resolve('Admin');
            });
        }
        else {

            return new Promise((resolve) => {
                this._salespersonManagementService.everyBody({
                    filter: {
                        logic: 'and',
                        filters: kendoFilter([
                            { field: 'SysKey', value: value, logic: 'and', operator: 'eq' }
                        ])
                    }
                }).pipe(takeUntil(this._unsubscribe))
                    .subscribe((data) => {
                        resolve(data[0]['Name'])
                    })
            })
        }
    }
}