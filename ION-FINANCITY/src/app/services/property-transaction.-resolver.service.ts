import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PropertyTransactionService } from './property-transaction.service';

@Injectable({ providedIn: 'root' })
export class PropertyTransactionResolverService implements Resolve<any>{

    constructor(private _propertyTransactionService: PropertyTransactionService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.paramMap.get('id') && route.paramMap.get('id') != null)
            return this._propertyTransactionService.findByKey(route.params.id)
        else
            return this._propertyTransactionService.default()
    }
}