import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ExternalSalespersonService } from './external-salesperson.service';

@Injectable({ providedIn: 'root' })
export class ExternalSalespersonResolverService implements Resolve<any>{

    constructor(private _externalSalespersonService: ExternalSalespersonService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.paramMap.get('id') && route.paramMap.get('id') != null)
            return this._externalSalespersonService.findByKey(route.params.id)
        else
            return this._externalSalespersonService.default()
    }
}