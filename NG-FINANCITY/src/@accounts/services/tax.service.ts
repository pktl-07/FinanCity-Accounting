import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { State } from '@progress/kendo-data-query';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { taxType, taxCategory } from 'src/@common/service/utility-service';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class TaxService extends BaseApiService {
    constructor(public http: HttpClient) {

        super(http, `Tax`);
    }

    getPager(state: State): void {

        this.subsink.sink = this.lazyLoader(state)
            .subscribe(x => super.next(x));
    }

    public lazyLoader(state: State): Observable<GridDataResult> {

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

    public transformData(data: []) {

        let result = [];
        for (let d of data) {

            result.push({
                SysKey: d['SysKey'],
                TaxCode: d['TaxCode'],
                Description: d['Description'],
                TaxPercent: d['TaxPercent'],
                InputTaxSK: `${(d['InputTaxSK'] as string).substr(0, 1)}-${(d['InputTaxSK'] as string).substr(1, 4)}`,
                OutputTaxSK: `${(d['OutputTaxSK'] as string).substr(0, 1)}-${(d['OutputTaxSK'] as string).substr(1, 4)}`,
                TaxType: taxType().filter(f => f['value'] == d['TaxType'])[0]['text'],
                TaxCategory: taxCategory().find(f => f['value'] == d['TaxCategory']).text,
                IsInclusive: d['IsInclusive'],
                IsDefault: d['IsDefault'],
                IsActive: d['IsActive'],
            });
        }
        return result;
    }
}