import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/utils';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class UtilityService {

    constructor(private _http: HttpClient) {
    }

    getGrossToNextPromotion() : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.get(`${environment.auth.api}/utility/getGrossToNextPromotion/${environment.auth.SPSK}`, { headers: headers })
    }

    getCloseSalesCommissionToDate() : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.get(`${environment.auth.api}/utility/getCloseSalesCommissionToDate/${environment.auth.SPSK}`, { headers: headers })
    }

    getCloseSalesCommissionToYear() : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.get(`${environment.auth.api}/utility/getCloseSalesCommissionToYear/${environment.auth.SPSK}`, { headers: headers })
    }

    getCloseSalesValueToDate() : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.get(`${environment.auth.api}/utility/getCloseSalesValueToDate/${environment.auth.SPSK}`, { headers: headers })
    }

    getCloseSalesValueToYear() : Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.get(`${environment.auth.api}/utility/getCloseSalesValueToYear/${environment.auth.SPSK}`, { headers: headers })
    }

    getPmt(param: { rate: number, months: number, loan: number }) : Observable<any> {
        
        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.get(`${environment.auth.api}/utility/pmt/${param.rate}/${param.months}/${param.loan}`, { headers: headers })
    }
}