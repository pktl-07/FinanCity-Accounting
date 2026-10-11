import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { map, tap } from 'rxjs/operators';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class LeadService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `Lead`);
    }

    asyncLeadShare(state: State = { filter: { filters: [], logic: "and" } }): void {

        this.loading = true;

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;

        this.subsink.sink = this.http.get(`${environment.auth.api}/DataList/GetLeadAndSalesperson/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<GridDataResult>{
                    data: response['Data'],
                    total: response['Total']
                })),
                tap(() => this.loading = false)
            ).subscribe(x => super.next(x));
    }
}