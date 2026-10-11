import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { GlobalConfigurationSubject } from 'src/app/mk-config';
import { Observable } from 'rxjs';
import { SubSink } from 'src/@common/service/utility-service';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationService implements OnDestroy {

    private _subsink = new SubSink();
    constructor(private http: HttpClient,
        private _global: GlobalConfigurationSubject) {
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    getChattingCount(): void {

        let headers = new HttpHeaders(getHeaderScheme());
        this._subsink.sink = this.http.get(`${environment.auth.api}/Notification/getChattingCount`, { headers: headers })
            .subscribe((data: number) => {
                this._global.ChattingCounter.next(data);
            })
    }

    getMortgageSalesCount(): void {

        let headers = new HttpHeaders(getHeaderScheme());
        this._subsink.sink = this.http.get(`${environment.auth.api}/Notification/getMortgageSalesCount`, { headers: headers })
            .subscribe((data: number) => {

                this._global.MortgageSalesCounter.next(data);
            })
    }

    getChatting(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/Notification/getChatting`, { headers: headers })
    }

    getMortgageSales(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/Notification/getMortgageSales`, { headers: headers })
    }

    hide(key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.put(`${environment.auth.api}/Notification/hide/${key}`, { headers: headers })
    }

    wipeSignalR(): void {

        let headers = new HttpHeaders(getHeaderScheme({ 'No-Auth': 'True' }));
        this._subsink.sink = this.http.delete(`${environment.auth.api}/Notification/wipeAll/1`, { headers: headers })
            .subscribe();
    }
}