import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/utils';
import { State } from '@progress/kendo-data-query';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class SalespersonManagementService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `SalespersonManagement`);
    }

    getSalespersonUpTree(spsk: string) : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getSalespersonUpTree/${spsk}`, { headers: headers })
          
    }

    asyncEveryBody(state: State) : void {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        this.subsink.sink = this.http.get(`${environment.auth.api}/utility/getEveryBody`, { headers: headers })
            .subscribe(x => super.next(x));
    }

    getCurrentCommSchemeByKey(spsk: string) : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCurrentCommissionSchemeByKey/${spsk}`, { headers: headers })
    }

    getCommissionSchemeByKey(spsk: string) : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCommissionSchemeByKey/${spsk}`, { headers: headers })
    }

    everyBody(state: State) : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        return this.http.get(`${environment.auth.api}/utility/getEveryBody`, { headers: headers })
    }

    isTierCommission(spsk: string) : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/isTierCommission/${spsk}`, { headers: headers })
    }

    senior(param: { spsk: string, rank: number }) : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/senior/${param.spsk}/${param.rank}`, { headers: headers })
    }
}