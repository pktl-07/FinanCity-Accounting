import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VendorService } from './vendor.service';

@Injectable({ providedIn: 'root' })
export class VendorResoverService implements Resolve<any>{

    constructor(private _vendor: VendorService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.paramMap.get('id') && route.paramMap.get('id') != null)
            return this._vendor.findByKey(route.params.id)
        else
            return this._vendor.default()
    }
}