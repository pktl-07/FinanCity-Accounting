import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { kendoFilter, SubSink } from '../utils';

@Pipe({
    name: 'fcName',
})
export class FCNamePipe implements PipeTransform, OnDestroy {

    public _subsink = new SubSink();

    constructor(private _salespersonManagementService: SalespersonManagementService) {
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
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
                this._subsink.sink = this._salespersonManagementService.everyBody({
                    filter: {
                        logic: 'and',
                        filters: kendoFilter([
                            { field: 'SysKey', value: value, logic: 'and', operator: 'eq' }
                        ])
                    }
                }).subscribe((data) => {
                    resolve(data[0]['Name'])
                })
            })
        }
    }
}