import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil, mergeMap } from 'rxjs/operators';
import { SalespersonManagementService } from '../salesperson-management.service';
import { CommissionStructureDetailService } from '../commission-structure-detail.service';
import { BranchOutService } from '../branch-out.service';

@Injectable({ providedIn: 'root' })
export class BranchOutDetailResolveService implements Resolve<any> {

    private _unsubscribe = new Subject();
    constructor(private _branchOutService: BranchOutService,
        private _salespersonManagementService: SalespersonManagementService,
        private _schemeDetailService: CommissionStructureDetailService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        if (route.params.id && route.params.id != null) {
            return this._branchOutService.findByKey(route.params.id)
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
                                }),
                                mergeMap(response => {
                                    return this._branchOutService.fullTree(route.params.id).pipe(takeUntil(this._unsubscribe),
                                        map(data => {

                                            response['nodes'] = data;
                                            return response;
                                        })
                                    )
                                })
                            )
                    })
                )
        } else {
            return this._branchOutService.default().pipe(takeUntil(this._unsubscribe),
                map(response => {

                    response['nodes'] = [];
                    response['NRIC'] = null;
                    response['Designation'] = null;

                    return response;
                })
            )
        }
    }
}