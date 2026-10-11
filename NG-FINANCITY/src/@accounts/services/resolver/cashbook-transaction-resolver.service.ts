import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CashbookTransactionService } from '../cashbook-transaction.service';
import { kendoFilter, multiCurrency } from 'src/@common/service/utility-service';
import { map, mergeMap } from 'rxjs/operators';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { TaxService } from '../tax.service';

@Injectable({ providedIn: 'root' })
export class CashbookTransactionResolverService implements Resolve<any>{

    constructor(private _cashbookTransactionService: CashbookTransactionService,
        private _currencyService: CurrencyService,
        private _taxService: TaxService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        let parentId = route.params.parentId;
        let type = route.params.type;

        if (parentId) {

            return this._cashbookTransactionService.finds({
                filter: {
                    logic: 'and',
                    filters: kendoFilter([{
                        field: 'ParentId', value: parentId, logic: 'and', operator: 'eq',
                    }, {
                        field: 'TransType', value: type, logic: 'and', operator: 'eq',
                    }])
                }
            }).pipe(
                map((response: any[]) => {

                    let d = {};

                    d['summary'] = response[0];

                    response.map(m => {
                        m['TaxAmount'] = multiCurrency({
                            value: m['TaxAmount'],
                            op: m['CURROP'],
                            rate: m['CURRATE'],
                        })

                        m['TotalAmount'] = multiCurrency({
                            value: m['TotalAmount'],
                            op: m['CURROP'],
                            rate: m['CURRATE'],
                        })

                        m['TransactionAmount'] = multiCurrency({
                            value: m['TransactionAmount'],
                            op: m['CURROP'],
                            rate: m['CURRATE'],
                        })
                    });

                    d['detail'] = response;

                    return d;
                }),
                mergeMap(response => {
                    return this._taxService.finds().pipe(
                        map((a: any[]) => {

                            response['tax'] = a;
                            return response;
                        })
                    );
                }),
                mergeMap((response: any) => {
                    return this._currencyService.findByKey(response['summary']['CURRSK']).pipe(
                        map((a: any) => {

                            response['summary']['CashbookCode'] = a['Currency_AlphaCode'];
                            return response;
                        })
                    )
                })
            );

        } else {
            return this._cashbookTransactionService.default().pipe(
                map((response: any[]) => {
                    let d = {};

                    d['summary'] = response;
                    d['detail'] = [];
                    d['summary']['CashbookCode'] = null;

                    return d;
                }),
                mergeMap(response => {
                    return this._taxService.finds().pipe(
                        map((a: any[]) => {

                            response['tax'] = a;
                            return response;
                        })
                    );
                }),
            )
        }
    }
}