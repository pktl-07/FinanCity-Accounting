import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { cloneData, kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';

@Injectable({ providedIn: 'root' })
export class LeadManagementDetailShareService implements OnDestroy {

    private _subsink = new SubSink();
    originSubject = new BehaviorSubject([]);
    salespersonClone = [];

    constructor(private _salespersonManagementService: SalespersonManagementService) {

        this._subsink.sink = this._salespersonManagementService.finds({
            filter: {
                logic: "and",
                filters: kendoFilter([
                    { field: 'IsActive', value: true, logic: 'and', operator: 'eq' }
                ])
            }
        }).subscribe((data: any[]) => {

            this.originSubject.next(data);
            this.salespersonClone = cloneData(data);
        })
    }
    ngOnDestroy(): void {
        this._subsink.unsubscribe();
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