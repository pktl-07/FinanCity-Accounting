import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { CommissionReceivedService } from '../commission-received.service';
import { kendoFilter } from 'src/@common/service/utility-service';
import { map, takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class CommissionReceiveAllocationResolveService implements Resolve<any> {

    private _unsubscribe = new Subject();

    constructor(private _commmissionReceivedService: CommissionReceivedService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        console.log(route.params.id)
        return this._commmissionReceivedService.pager({
            skip: 0,
            take: 1,
            filter: {
                filters: kendoFilter([
                    { field: 'InvoiceSK', value: route.params.id, operator: 'eq', logic: 'and' }]),
                logic: 'and'
            }
        }).pipe(
            takeUntil(this._unsubscribe),
            map(data => data.data[0]));
    }
}