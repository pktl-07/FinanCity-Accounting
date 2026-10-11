import { Injectable, Inject } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { State } from '@progress/kendo-data-query';
import { tap, mergeMap, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class SalespersonManagementService extends BaseApiService {

    constructor(public http: HttpClient) {

        super(http, `SalespersonManagement`);
    }

    //to find out every body it means not only internal salesperson but also external salesperson or clients ect..
    asyncEveryBody(state: State) {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        this.subsink.sink = this.http.get(`${environment.auth.api}/utility/getEveryBody`, { headers: headers })
            .pipe(
                map(response => response),
                tap(() => this.loading = false)
            ).subscribe(x => super.next(x));
    }

    asyncEveryClient(person: State,
        vendor: State): void {

        this.loading = true;

        let sp = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(person.filter) }));
        let v = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(vendor.filter) }));

        this.subsink.sink = this.http.get(`${environment.auth.api}/utility/getEveryBody`, { headers: sp })
            .pipe(
                map(response => response),
                mergeMap((response: any[]) => {
                    return this.http.get(`${environment.auth.api}/Vendor/get`, { headers: v })
                        .pipe(map((m: any[]) => {
                            m.forEach(a => {
                                response.push({ SysKey: a['SysKey'], Name: a['CompanyName'] })
                            })
                            return response;
                        }))
                }),
                tap(() => this.loading = false)
            ).subscribe(x => super.next(x));
    }

    schemeHeaderKey(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/schemeHeaderKey/${spsk}`, { headers: headers })
    }

    everyBody(state: State): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        return this.http.get(`${environment.auth.api}/utility/getEveryBody`, { headers: headers })
    }

    getSalespersonUpTree(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getSalespersonUpTree/${spsk}`, { headers: headers })
    }

    getSalespersonFullTree(spks: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getSalespersonFullTree/${spks}`, { headers: headers })
    }

    getSalespersonDownTree(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getSalespersonDownTree/${spsk}`, { headers: headers })
    }


    getCloseSalesCommissionToDate(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCloseSalesCommissionToDate/${spsk}`, { headers: headers })
    }

    getCloseSalesCommissionToYear(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCloseSalesCommissionToYear/${spsk}`, { headers: headers })
    }

    getCloseSalesValueToDate(spsk: string) {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCloseSalesValueToDate/${spsk}`, { headers: headers })
    }

    getCloseSalesValueToYear(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCloseSalesValueToYear/${spsk}`, { headers: headers })
    }

    getCurrentCommSchemeByKey(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCurrentCommissionSchemeByKey/${spsk}`, { headers: headers })
    }

    getCommissionSchemeByKey(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCommissionSchemeByKey/${spsk}`, { headers: headers })
    }

    getGrossToNextPromotion(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getGrossToNextPromotion/${spsk}`, { headers: headers })
    }

    getCommissionReceivedDTO(state: State): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/getCommissionReceivedDTO/${JSON.stringify(state.filter)}`, { headers: headers })
    }

    isTierCommission(spsk: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/isTierCommission/${spsk}`, { headers: headers })
    }

    getSenior(spsk: string, rank: number) {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.get(`${environment.auth.api}/utility/senior/${spsk}/${rank}`, { headers: headers })
    }
}