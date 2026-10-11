import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme, SubSink } from 'src/@common/utils';
import { DataResult, State } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationService extends BehaviorSubject<any> implements OnDestroy {

    private _subsink = new SubSink();
    constructor(
        private _http: HttpClient) {

        super([])
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    subscribeChattingCount(): void {

        let headers = new HttpHeaders(getHeaderScheme());
        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/getChattingCount`, { headers: headers })
            .subscribe((data: number) => {

                environment.general.chattingCounter.next(data);
            })
    }

    subscribeMortgageSalesCount(): void {

        let headers = new HttpHeaders(getHeaderScheme());
        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/getMortgageSalesCount`, { headers: headers })
            .subscribe((data: number) => {
                environment.general.mortageSalesCounter.next(data);
            })
    }

    subscribeLeadFollowUpCount(): void {

        let headers = new HttpHeaders(getHeaderScheme());
        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/getLeadFollowUpCount`, { headers: headers })
            .subscribe((data: number) => {
                environment.general.leadFollowUpCounter.next(data);
            })
    }

    wipeSignalR(param: { url: string, spsk: string }): void {

        let headers = new HttpHeaders(getHeaderScheme({ 'No-Auth': 'True' }));
        this._subsink.sink = this._http.delete(`${param.url}/Notification/wipeAll/${param.spsk}`, { headers: headers })
            .subscribe();
    }

    getLeadFollowUp(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this._http.get(`${environment.auth.api}/Notification/getLeadFollowUp`, { headers: headers });
    }

    getChatting(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this._http.get(`${environment.auth.api}/Notification/getChatting`, { headers: headers })
    }

    getMortgageSales(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this._http.get(`${environment.auth.api}/Notification/getMortgageSales`, { headers: headers })
    }

    subScribeAndriodVersion(): void {

        let headers = new HttpHeaders(getHeaderScheme());
        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/getAndriodVersion`, { headers: headers })
            .subscribe((version: number) => { environment.general.version.next(version) })
    }

    subscribeIOSVersion(): void {

        let headers = new HttpHeaders(getHeaderScheme());
        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/getIOSVersion`, { headers: headers })
            .subscribe((version: number) => { environment.general.version.next(version) })
    }

    asyncMortgageSalesPager(state: State = {
        filter: {
            filters: [],
            logic: 'and'
        },
        skip: 0,
        take: 14
    }): void {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;
        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/mortgagePager/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<DataResult>{
                    data: response['Data'],
                    total: response['Total']
                }))
            ).subscribe(x => super.next(x));
    }

    asyncChattingPager(state: State = {
        filter: {
            filters: [],
            logic: 'and'
        },
        skip: 0,
        take: 14
    }): void {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;

        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/chattingPager/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<DataResult>{
                    data: response['Data'],
                    total: response['Total']
                }))
            ).subscribe(x => super.next(x));
    }

    asyncFollowUpPager(state: State = {
        filter: {
            filters: [],
            logic: 'and'
        },
        skip: 0,
        take: 14
    }): void {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;

        this._subsink.sink = this._http.get(`${environment.auth.api}/Notification/followUpPager/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<DataResult>{
                    data: response['Data'],
                    total: response['Total']
                }))
            ).subscribe(x => super.next(x));
    }

    hide(key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this._http.put(`${environment.auth.api}/Notification/hide/${key}`, { headers: headers })
    }
}