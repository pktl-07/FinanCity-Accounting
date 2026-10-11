import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CommissionReceivedService } from '../commission-received.service';
import { kendoFilter } from 'src/@common/service/utility-service';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class CommissionReceivedResolverService implements Resolve<any>{

    constructor(private _commmissionReceivedService: CommissionReceivedService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        return this._commmissionReceivedService.pager({
            skip: 0,
            take: 1,
            filter: {
                filters: kendoFilter([
                    { field: 'InvoiceSK', value: route.params.id, operator: 'eq', logic: 'and' }
                ]),
                logic: 'and'
            }
        }).pipe(map(data => data.data[0]));
    }
}