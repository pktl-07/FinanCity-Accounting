import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class CashbookTransferService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `CashbookTransfer`);
    }

    recap(parentId: string): Observable<any> {
        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/CashbookTransfer/recap/${parentId}`, { headers: headers })
    }
}