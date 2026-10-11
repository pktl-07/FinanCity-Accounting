import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil, mergeMap } from 'rxjs/operators';
import { ChangeOfStatusService } from '../change-of-status.service';
import { SalespersonManagementService } from '../salesperson-management.service';
import { CommissionStructureDetailService } from '../commission-structure-detail.service';

@Injectable({ providedIn: 'root' })
export class ChangeOfStatusResolveService implements Resolve<any> {

    private _unsubscribe = new Subject();
    constructor(private _changeOfStatusService: ChangeOfStatusService,
        private _salespersonManagementService: SalespersonManagementService,
        private _schemeDetailService: CommissionStructureDetailService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.params.id && route.params.id != null) {
            return this._changeOfStatusService.findByKey(route.params.id)
                .pipe(takeUntil(this._unsubscribe),
                    mergeMap(response => {

                        return this._salespersonManagementService.findByKey(response.SPSK)
                            .pipe(takeUntil(this._unsubscribe),
                                map(sp => {

                                    response['NRIC'] = sp['Person']['NRIC'];
                                    return response;
                                }),
                                mergeMap(response => {
                                    return this._schemeDetailService.findByKey(response['SchemeDetailSK'])
                                        .pipe(takeUntil(this._unsubscribe),
                                            map(data => {
                                                response['Designation'] = data['Designation'];
                                                return response;
                                            })
                                        )
                                })
                            )
                    })
                )
        } else {
            return this._changeOfStatusService.default().pipe(takeUntil(this._unsubscribe),
                map(response => {

                    response['NRIC'] = null;
                    response['Designation'] = null;

                    return response;
                })
            )
        }
    }
}