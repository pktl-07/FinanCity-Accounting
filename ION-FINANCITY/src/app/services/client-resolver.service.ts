import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { ClientService } from './client-service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ClientResolverService implements Resolve<any>{

    constructor(private _clientService: ClientService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot,): Observable<any> | Promise<any> | any {

        if (route.paramMap.get('id') && route.paramMap.get('id') != null)
            return this._clientService.findByKey(route.params.id)
        else
            return this._clientService.default()
    }
}