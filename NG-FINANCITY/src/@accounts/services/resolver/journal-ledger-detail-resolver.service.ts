import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { JournalLedgerService } from '../journal-ledger-service';
import { kendoFilter } from 'src/@common/service/utility-service';
import { map, mergeMap } from 'rxjs/operators';
import { AccountSetupService } from '../account-setup.service';
import { TaxService } from '../tax.service';
import { JobService } from '../job.service';

@Injectable({ providedIn: 'root' })
export class JournalLedgerDetailResolverService implements Resolve<any>{

    constructor(private _journalLedgerService: JournalLedgerService,
        private _accountSetupService: AccountSetupService,
        private _taxService: TaxService,
        private _jobService: JobService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        let parentId = route.params.parentId;
        let moduleCode = route.params.moduleCode;
        let postDate = route.params.postDate;

        if (parentId && moduleCode && postDate) {
            return this._journalLedgerService.getJournalLedgerSummary({
                filter: {
                    filters: kendoFilter([
                        { field: 'ParentId', value: parentId, logic: 'and', operator: 'eq' },
                        { field: 'ModuleCode', value: moduleCode, logic: 'and', operator: 'eq' },
                        { field: 'PostDate', value: postDate, logic: 'and', operator: 'eq' },
                    ]),
                    logic: 'and',
                }
            }).pipe(
                map((response: any[]) => {
                    const d = {};
                    d['summary'] = response[0];
                    d['summary']['PostDate'] = postDate;
                    return d;
                }),
                mergeMap((response) => {
                    return this._accountSetupService.finds().pipe(
                        map(a => {
                            response['accounts'] = a;
                            return response;
                        })
                    );
                }),
                mergeMap(response => {
                    return this._taxService.finds().pipe(
                        map((a: any[]) => {

                            response['tax'] = a;
                            return response;
                        })
                    );
                }),
                mergeMap(response => {
                    return this._jobService.finds().pipe(
                        map((a: any[]) => {

                            response['job'] = a;
                            return response;
                        })
                    );
                })
            );
        }
        else {

            return this._journalLedgerService.default().pipe((map(response => {
                const d = {};
                d['summary'] = response;
                return d;
            })),
                mergeMap((response) => {
                    return this._accountSetupService.finds().pipe(
                        map(a => {
                            response['accounts'] = a;
                            return response;
                        })
                    );
                }),
                mergeMap(response => {
                    return this._taxService.finds().pipe(
                        map((a: any[]) => {
                            response['tax'] = a;
                            return response;
                        })
                    );
                }),
                mergeMap(response => {
                    return this._jobService.finds().pipe(
                        map((a: any[]) => {
                            response['job'] = a;
                            return response;
                        })
                    );
                })

            );
        }
    }
}