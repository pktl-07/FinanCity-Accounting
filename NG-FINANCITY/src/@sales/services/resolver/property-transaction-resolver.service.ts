import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { PropertyTransactionService } from '../property-transaction.service';
import { map, takeUntil, mergeMap } from 'rxjs/operators';
import { multiCurrency } from 'src/@common/service/utility-service';
import { CommissionStructureDetailService } from '../commission-structure-detail.service';


@Injectable({ providedIn: 'root' })
export class PropertyTransactionResolverService implements Resolve<any> {

    private _unsubscribe = new Subject();

    constructor(private _propertyTransactionService: PropertyTransactionService,
        private _commissionStructure: CommissionStructureDetailService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.params.id && route.params.id != null)
            return this._propertyTransactionService.findByKey(route.params.id)
                .pipe(
                    takeUntil(this._unsubscribe),
                    map(data => {

                        data['TNC']['LoanAmount'] = multiCurrency({
                            op: data['TNC']['CURROP'],
                            rate: data['TNC']['CURRATE'],
                            value: data['TNC']['LoanAmount']
                        });

                        data['PropertyTransaction']['TransactedPrice'] = multiCurrency({
                            op: data['TNC']['CURROP'],
                            rate: data['TNC']['CURRATE'],
                            value: data['PropertyTransaction']['TransactedPrice']
                        });

                        data['PropertyTransaction']['TaxAmount'] = multiCurrency({
                            op: data['PropertyTransaction']['CURROP'],
                            rate: data['PropertyTransaction']['CURRATE'],
                            value: data['PropertyTransaction']['TaxAmount']
                        });

                        data['PropertyTransaction']['TotalAmount'] = multiCurrency({
                            op: data['PropertyTransaction']['CURROP'],
                            rate: data['PropertyTransaction']['CURRATE'],
                            value: data['PropertyTransaction']['TotalAmount']
                        });

                        data['PropertyTransaction']['GrossCommission'] = multiCurrency({
                            op: data['PropertyTransaction']['CURROP'],
                            rate: data['PropertyTransaction']['CURRATE'],
                            value: data['PropertyTransaction']['GrossCommission']
                        });

                        return data;
                    }), mergeMap((response: any[]) => {
                        return this.getCommissionSource(response);
                    })
                );
        else
            return this._propertyTransactionService.default()
                .pipe(mergeMap((response: any) => {
                    return this.getCommissionSource(response);
                }));
    }

    private getCommissionSource(data: any) {

        return this._commissionStructure.finds().pipe(takeUntil(this._unsubscribe),
            map((response: any[]) => {
                response.map((m => {
                    if (m['Type'] == 4)
                        response['Designation'] = 'TIER';
                }))
                data['CommissionSource'] = response;
                return data;
            }));
    }
}



