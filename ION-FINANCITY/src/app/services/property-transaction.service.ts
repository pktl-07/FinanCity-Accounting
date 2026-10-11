import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/utils';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class PropertyTransactionService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `PropertyTransaction`);
    }

    saveDraft(data: {}): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.post(`${environment.auth.api}/PropertyTransaction/savedraft`, data, { headers: headers })
    }

    submit(data: {}): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.post(`${environment.auth.api}/PropertyTransaction/submit`, data, { headers: headers })
    }
}