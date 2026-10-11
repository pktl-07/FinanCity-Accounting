import { Injectable, Inject } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class PropertyTransactionService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `PropertyTransaction`);
    }

    approve(data: any): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.put(`${environment.auth.api}/${this.route}/approve`, data, { headers: headers })
    }

    reject(data: any): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.put(`${environment.auth.api}/${this.route}/reject`, data, { headers: headers })
    }
}