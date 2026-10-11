import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { State } from '@progress/kendo-data-query';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map, tap } from 'rxjs/operators';
import { kendoFilter } from 'src/@common/service/utility-service';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class CommissionPayableService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `CommissionPayment`);
    }

    getDetailPager(state: State = null): void {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;

        this.subsink.sink = this.http.get(`${environment.auth.api}/utility/GetCommissionPayableDetailPager/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<GridDataResult>{
                    data: response['Data'],
                    total: response['Total']
                })),
                tap(() => this.loading = false)
            ).subscribe(x => this.next(x));
    }

    recap(key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/CommissionPayment/recap/${key}`, { headers: headers })
    }

    getSalespersonBalance(alosk: string): Observable<any> {

        let state: State = {
            take: 10, skip: 0, filter: {
                filters: kendoFilter([{ field: 'ALOSK', value: alosk, operator: 'eq', logic: 'and' }])
                , logic: 'and'
            }
        }
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;
        return this.http.get(`${environment.auth.api}/utility/GetCommissionPayableDetailPager/${currentPage}/${state.take}`,
            { headers: headers }).pipe(
                map(response => (<GridDataResult>{
                    data: response['Data'],
                    total: response['Total']
                })),
                map(response => {
                    return response['data'][0]['PayableAmount'] - response['data'][0]['PaidAmount'];
                })
            );
    }
}
