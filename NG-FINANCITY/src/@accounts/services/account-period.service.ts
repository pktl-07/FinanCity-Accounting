import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { map, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class AccountPeriodService extends BaseApiService {

    constructor(public http: HttpClient) {

        super(http, `AccountPeriod`);
    }

    asyncGetAccountingPeriod(yc: string): void {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({}));
        this.subsink.sink = this.http.get(`${environment.auth.api}/utility/getAccountingPeriodDTO/${yc}`, { headers: headers })
            .pipe(
                map(response => response),
                tap(() => this.loading = false)
            ).subscribe(x => super.next(x));
    }

    save(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.post(`${environment.auth.api}/${this.route}/save`, {}, { headers: headers })
    }

    update(data: any): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.put(`${environment.auth.api}/${this.route}/update`, data, { headers: headers });
    }

    getLastAccountingYearCode(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getLastAccountingYearCode`, { headers: headers });
    }

    getLastAccountingYear(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getLastAccountingYear`, { headers: headers });
    }

    getCurrentAccountingYearCode(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCurrentAccountingYearCode`, { headers: headers });
    }

    getCurrentAccountingYear(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCurrentAccountingYear`, { headers: headers });
    }
}