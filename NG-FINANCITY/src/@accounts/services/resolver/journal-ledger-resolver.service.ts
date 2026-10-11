import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountPeriodService } from '../account-period.service';

@Injectable({ providedIn: 'root' })
export class JournalLedgerResolverService implements Resolve<any>{

    constructor(private _accountingPeriodService: AccountPeriodService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        return this._accountingPeriodService.getCurrentAccountingYear();
    }
}