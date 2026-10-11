import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme, SubSink } from 'src/@common/utils';
import { State, DataResult } from '@progress/kendo-data-query';
import { BehaviorSubject, Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class DataListService extends BehaviorSubject<any> implements OnDestroy {

    private _subsink = new SubSink();
    completed = new Subject();
    constructor(
        private _http: HttpClient) {

        super(null);
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    getSalespersonCommissionPager(state: State = {
        filter: {
            filters: [],
            logic: 'and'
        },
        skip: 0,
        take: 14
    }): void {

        let currentPage = (state.skip / state.take) + 1;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        this._subsink.sink = this._http.get(`${environment.auth.api}/DataList/GetSalespersonCommissionPager/${currentPage}/${state.take}`, { headers: headers })
            .pipe(
                map(response => (<DataResult>{
                    data: response['Data'],
                    total: response['Total']
                }))
            ).subscribe(data => {
                this.next(data);
            });
    }
}