import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SalespersonManagementService } from './salesperson-management.service';
import { mergeMap, map } from 'rxjs/operators';
import { UtilityService } from './util.service';
import { CommissionStructureService } from './commission-structure.service';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class MyProfileResolverService implements Resolve<any>{

    constructor(private _salespersonManagementService: SalespersonManagementService,
        private _utilityService: UtilityService,
        private _commissionStructureService: CommissionStructureService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        return this._salespersonManagementService
            .findByKey(environment.auth.SPSK)
            .pipe(
                mergeMap(data => {
                    return this._utilityService.getGrossToNextPromotion()
                        .pipe(
                            map(response => {
                                data['Employment']['GrossToPromoteNext'] = response;
                                return data;
                            }))
                }),
                mergeMap(data => {

                    return this._utilityService.getCloseSalesCommissionToDate()
                        .pipe(
                            map(response => {
                                data['Employment']['CloseSalesCommissionToDate'] = response;
                                return data;
                            }))
                }),
                mergeMap(data => {

                    return this._utilityService.getCloseSalesCommissionToYear()
                        .pipe(
                            map(response => {
                                data['Employment']['CloseSalesCommissionToYear'] = response;
                                return data;
                            }))
                }),
                mergeMap(data => {

                    return this._utilityService.getCloseSalesValueToDate()
                        .pipe(
                            map(response => {
                                data['Employment']['CloseSalesValueToDate'] = response;
                                return data;
                            }))
                }),
                mergeMap(data => {

                    return this._utilityService.getCloseSalesValueToYear()
                        .pipe(
                            map(response => {
                                data['Employment']['CloseSalesValueToYear'] = response;
                                return data;
                            }))
                }),
                mergeMap(data => {
                    return this._salespersonManagementService.getCurrentCommSchemeByKey(environment.auth.SPSK)
                        .pipe(
                            map(response => {
                                data['Employment']['SchemeSK'] = response['ParentSK'];
                                data['Employment']['Designation'] = response['Designation'];
                                data['Employment']['SalespersonPayout'] = `${response['AssociatedPercent']}/${response['AgencyPercent']}`;
                                return data;
                            }))
                }), mergeMap(data => {

                    return this._commissionStructureService.findByKey(data['Employment']['SchemeSK'])
                        .pipe(
                            map(response => {
                                data['Employment']['CommissionScheme'] = response.CommissionScheme.SchemeName;
                                return data;
                            }))
                }),
                mergeMap(data => {
                    return this._salespersonManagementService.senior({ spsk: environment.auth.SPSK, rank: 10 })
                        .pipe(
                            map(director => {
                                data['Employment']['Director'] = director;
                                return data;
                            }),
                            mergeMap((response: any) => {

                                return this._salespersonManagementService.senior({ spsk: environment.auth.SPSK, rank: 5 })
                                    .pipe(
                                        map(manager => {
                                            data['Employment']['Manager'] = manager;
                                            return response;
                                        }))
                            })
                        )
                })
            );
    }
}
