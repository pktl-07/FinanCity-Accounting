import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { cloneData } from 'src/@common/common.constant';
import { kendoFilter, SubSink } from 'src/@common/utils';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { environment } from 'src/environments/environment';

export interface Salesperson {
    SysKey: string,
    Name: string,
}

@Injectable({ providedIn: 'root' })
export class ShareContactService implements OnDestroy {

    originSubject: BehaviorSubject<Array<Salesperson>> = new BehaviorSubject([]);
    salespersonClone = [];

    private _subsink = new SubSink();
    constructor(private _salespersonManagementService: SalespersonManagementService) {

        this.poppulateSalespersonList();
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    poppulateSalespersonList(): void {

        this._subsink.sink = this._salespersonManagementService.finds({
            filter: {
                logic: "and",
                filters: kendoFilter([
                    { field: 'IsActive', value: true, logic: 'and', operator: 'eq' },
                    { field: 'SysKey', value: environment.auth.SPSK, logic: 'and', operator: 'neq' },
                ])
            }
        }).subscribe((data: Array<Salesperson>) => {
            this.originSubject.next(data);
            this.salespersonClone = cloneData(data);
        })
    }

    getInsertedItem(param: {
        originKeys: string[],
        selectedKeys: string[]
    }) {

        return param.selectedKeys.filter((f) => !param.originKeys.includes(f));
    }

    getDeletedItem(param: {
        originKeys: string[],
        selectedKeys: string[]
    }) {

        return param.originKeys.filter((f) => !param.selectedKeys.includes(f));
    }
}