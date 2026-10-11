import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LeadService } from './lead.service';

@Injectable({ providedIn: 'root' })
export class LeadResolverService implements Resolve<any>{

    constructor(private _leadService: LeadService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.paramMap.get('id') && route.paramMap.get('id') != null)
            return this._leadService.findByKey(route.params.id)
        else
            return this._leadService.default()
    }
}