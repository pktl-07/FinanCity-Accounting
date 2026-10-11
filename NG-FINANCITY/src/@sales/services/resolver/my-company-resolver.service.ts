import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { MyCompanyService } from '../my-company.service';
import { takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class MyCompanyResolverService implements Resolve<any> {

    constructor(private _myCompanyservice: MyCompanyService) {
    }

    private _unsubscrbe = new Subject();

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        return this._myCompanyservice.findByKey("1").pipe(takeUntil(this._unsubscrbe));
    }
}