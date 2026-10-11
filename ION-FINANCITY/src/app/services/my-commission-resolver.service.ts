import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable, } from '@angular/core';
import { Observable } from 'rxjs';
import { kendoFilter, getHeaderScheme } from 'src/@common/utils';
import { DataListService } from './data-list.service';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { State } from '@progress/kendo-data-query';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class MyCommissionResolverService implements Resolve<any>{

    constructor(public _dataListService: DataListService,
        private _http: HttpClient) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        let s: State = {
            filter: {
                filters: kendoFilter([
                    { field: 'PropertySK', value: route.paramMap.get('id'), operator: 'eq', logic: 'and' },
                    { field: 'SPSK', value: environment.auth.SPSK, operator: 'eq', logic: 'and' }
                ]),
                logic: 'and',
            },
            skip: 1,
            take: 14
        }

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(s.filter) }));
        return this._http.get(`${environment.auth.api}/DataList/GetSalespersonCommissionPager/${s.skip}/${s.take}`,
            { headers: headers })
            .pipe(
                map(response => {
                    let r = response['Data'][0];
                    r['Balance'] = r['PayableAmount'] - r['AmountPaid'];
                    return r;
                })
            )
    }
}