import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class ChattingRemarkService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `ChattingRemark`);
    }

    last(refsk: string) {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme());

        return this._http.get(`${environment.auth.api}/utility/lastRemark/${refsk}`,
            { headers: headers });
    }
}