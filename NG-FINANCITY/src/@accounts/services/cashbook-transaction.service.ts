import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class CashbookTransactionService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `CashbookTransaction`);
    }

    recap(param: { parentId: string, type: number }): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/CashbookTransaction/recap/${param.parentId}/${param.type}`, { headers: headers })
    }
}