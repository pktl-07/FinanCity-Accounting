import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { State } from '@progress/kendo-data-query';
import { kendoFilter, multiCurrency } from 'src/@common/service/utility-service';
import { tap, map } from 'rxjs/operators';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class JournalLedgerService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `JournalLedger`);
    }

    reverseGL(e: { parentId: string, postDate: number }): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.put(`${environment.auth.api}/JournalLedger/reverse/${e.parentId}/${e.postDate}`, { headers: headers })
    }

    voidGL(e: { parentId: string, postDate: number }): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.put(`${environment.auth.api}/JournalLedger/void/${e.parentId}/${e.postDate}`, { headers: headers })
    }

    getAuditPager(state: State = null): void {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;

        this.subsink.sink = this.http.get(`${environment.auth.api}/DataList/GetAuditPager/${currentPage}/${state.take}`,
            { headers: headers }).pipe(
                map(response => (<GridDataResult>{
                    data: response['Data'],
                    total: response['Total']
                })),
                tap(() => this.loading = false)
            ).subscribe(pager => {
                this.next(pager);
            });
    }

    getJournalLedgerSummary(state: State = { filter: { filters: [], logic: "and" } }): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        return this.http.get(`${environment.auth.api}/DataList/GetJournalLedgerSummary`, { headers: headers })
    }

    recap(e: { parentId: string, moduleCode: string, postDate: number }): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/JournalLedger/recap/${e.parentId}/${e.moduleCode}/${e.postDate}`, { headers: headers })
    }

    asyncFindGL(e: { ycpc: string, moduleCode: string, parentId: string, postDate: number }): void {

        let state: State = {
            filter: {
                filters: kendoFilter([
                    { field: 'ParentId', value: e.parentId, logic: 'and', operator: 'eq' },
                    { field: 'ModuleCode', value: e.moduleCode, logic: 'and', operator: 'eq' },
                    { field: 'YCPC', value: e.ycpc, logic: 'and', operator: 'startswith' },
                    { field: 'PostDate', value: e.postDate, logic: 'and', operator: 'eq' },
                ]),
                logic: 'and'
            }
        }

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        this.loading = true;

        this.subsink.sink = this.http.get(`${environment.auth.api}/${this.route}/get`,
            { headers: headers })
            .pipe(map((response: any[]) => {

                response.map(m => {
                    m['Editable'] = true;
                    if (m.TransactionAmount > 0) {
                        m['Debit'] = multiCurrency({
                            value: m['TransactionAmount'],
                            rate: m['CURRATE'],
                            op: m['CURROP']
                        });
                        m['Credit'] = 0;
                    } else {
                        m['Debit'] = 0;
                        m['Credit'] = multiCurrency({
                            value: m.TransactionAmount * (-1),
                            rate: m['CURRATE'],
                            op: m['CURROP']
                        });
                    }

                    m['TaxAmount'] = multiCurrency({
                        value: m.TaxAmount,
                        rate: m['CURRATE'],
                        op: m['CURROP']
                    });
                })
                return response;
            }),
                map((response: any[]) => {
                    response.map((m, i) => {
                        if (m['HasTaxLine'] == true) {

                            let taxLine = response[i + 1];

                            m['TaxCode'] = taxLine['TaxCode'];
                            m['TaxPercent'] = taxLine['TaxPercent'];
                            m['TaxSK'] = taxLine['TaxSK'];
                            m['Editable'] = true;

                            taxLine['TaxCode'] = 'N/A';
                            taxLine['TaxPercent'] = 0;
                            taxLine['TaxSK'] = null;
                            taxLine['Editable'] = false;

                        }
                    })
                    return response;
                }),
                tap(() => this.loading = false)
            ).subscribe(data => this.next(data));
    }
}