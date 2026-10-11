import { Injectable, Inject } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class CommissionReceivedAllocationService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `CommissionReceivedAllocation`);
    }

    SalespersonTotalReceivable(spsk: string, invoicesk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/SalespersonTotalReceivable/${spsk}/${invoicesk}`, { headers: headers })
    }

    SalespersonTotalReceived(spsk: string, invoicesk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/SalespersonTotalReceived/${spsk}/${invoicesk}`, { headers: headers })
    }

    recap(key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/CommissionReceivedAllocation/recap/${key}`, { headers: headers })
    }
}