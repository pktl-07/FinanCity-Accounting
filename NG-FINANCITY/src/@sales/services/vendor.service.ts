import { Injectable, Inject } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { State } from '@progress/kendo-data-query/dist/npm/state';
import { Observable } from 'rxjs';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map, tap } from 'rxjs/operators';
import { vendorType } from 'src/@common/service/utility-service';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class VendorService extends BaseApiService {

    constructor(private http: HttpClient) {
        super(http, `Vendor`);
    }

    getPager(state: State): void {

        this.subsink.sink = this.lazyLoader(state)
            .subscribe(x => super.next(x));
    }

    private lazyLoader(state: State): Observable<GridDataResult> {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;

        return this.http.get(`${environment.auth.api}/${this.route}/pager/${currentPage}/${state.take}`,
            { headers: headers }).pipe(
                map(response => (<GridDataResult>{
                    data: this.transformData(response['Data']),
                    total: response['Total']
                })),
                tap(() => this.loading = false)
            );
    }

    private transformData(data: []) {

        let type = vendorType();
        let result = [];
        let vt = "";

        for (let d of data) {

            let f = type.filter(f => f.value == d['VendorType']);
            if (f.length > 0)
                vt = f[0].text;
            else
                vt = "";

            result.push({
                SysKey: d['SysKey'],
                VendorNo: d['VendorNo'],
                VendorType: vt,
                CompanyName: d['CompanyName'],
                Address: d['Address'],
                ACRANo: d['ACRANo'],
                IsActive: d['IsActive'],
                ContactPerson: d['ContactPerson'],
            });
        }
        return result;
    }
}