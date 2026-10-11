import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ReferralSalesOrderService } from '../referral-sales-order.service';
import { map, mergeMap } from 'rxjs/operators';
import { multiCurrency, addressFormat, kendoFilter } from 'src/@common/service/utility-service';
import { ContactService } from '../contact.service';
import { SalespersonManagementService } from '../salesperson-management.service';

@Injectable({ providedIn: 'root' })
export class ReferralSalesOrderResolverService implements Resolve<any> {

    constructor(private _referralSaleOrderService: ReferralSalesOrderService,
        private _personService: SalespersonManagementService,
        private _contactService: ContactService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.params.id && route.params.id != null)
            return this._referralSaleOrderService.findByKey(route.params.id).pipe(
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
                }),
                mergeMap((response: any) => {
                    return this._contactService.address(response['Client']['ClientSK'])
                        .pipe(map((contact: any) => {

                            response['Client']['ClientAddress'] = contact;
                            return response;
                        }))
                }),
                mergeMap((response: any) => {

                    return this._personService.everyBody({
                        filter: {
                            logic: 'and',
                            filters: kendoFilter([
                                { field: 'SysKey', value: response['Client']['ClientSK'], logic: 'and', operator: 'eq' }
                            ])
                        }
                    }).pipe(map((data) => {

                        response['Client']['ClientNRIC'] = data[0]['NRIC'];
                        return response;
                    }))
                })

            );
        else
            return this._referralSaleOrderService.default()
                .pipe(map((response: any) => {
                    response['Client']['ClientAddress'] = null;
                    response['Client']['ClientNRIC'] = null;
                    return response;
                }));
    }
}



