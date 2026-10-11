import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SalesInvoiceService } from '../sales-invoice.service';
import { map, mergeMap } from 'rxjs/operators';
import { PropertyTransactionService } from '../property-transaction.service';

@Injectable({ providedIn: 'root' })
export class SalesInvoiceResolverService implements Resolve<any>{

    constructor(private _salesInvoiceService: SalesInvoiceService,
        private _propertyTransactionService: PropertyTransactionService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        return this._salesInvoiceService.findByKey(route.params.id)
            .pipe(mergeMap(response => {
                return this._propertyTransactionService.findByKey(response.SalesInvoice.SysKey)
                    .pipe(map(prop => {
                        response['PropertyTransaction'] = prop['PropertyTransaction'];
                        return response;
                    }))
            }))
    }
}