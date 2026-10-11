import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class MarketingExpenseService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `MarketingExpense`);
    }

    getMarketingExpense(spsk: string) {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/GetMarketingExpense/${spsk}`, { headers: headers })
    }
}