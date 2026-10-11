import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/utils';
import { State, DataResult } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class ClientService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `Client`);
    }

    clientAndLeadPager(state: State = {
        filter: {
            filters: [],
            logic: 'and'
        },
        skip: 1,
        take: 14
    }): void {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;
        this.subsink.sink = this.http.get(`${environment.auth.api}/DataList/ClientAndLeadPager/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<DataResult>{
                    data: response['Data'],
                    total: response['Total']
                }))
            ).subscribe(x => super.next(x));;
    }
}