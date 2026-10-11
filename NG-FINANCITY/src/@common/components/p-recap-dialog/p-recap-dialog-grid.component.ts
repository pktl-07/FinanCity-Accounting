import { Component, ViewChild, OnInit, OnDestroy, Input, Injectable, ViewEncapsulation, Inject } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { PGridViewComponent } from '../p-grid-view/p-grid-view.component';
import { map, mergeMap, finalize } from 'rxjs/operators';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { kendoFilter, cloneData, multiCurrency, SubSink } from 'src/@common/service/utility-service';
import { environment } from 'src/environments/environment';

@Injectable()
export class RecapGridSubject extends BehaviorSubject<any>{

    loading: boolean;

    constructor() {
        super([]);
    }
}

@Component({
    encapsulation: ViewEncapsulation.None,
    selector: 'recap-dialog-grid',
    template: `
    <div class="fc-grid">
        <kendo-grid [data]="view | async" [loading]="view?.loading" [style.height.%]="100">
            <kendo-grid-column field="AccountCode" title="Account Code" 
                width="150"
                [headerClass]="{'text-center': true}" 
                [class]="{'text-center':true}">
            </kendo-grid-column>
            <kendo-grid-column field="Account" title="Account" [footerClass]="{'text-right':true}">
                <ng-template kendoGridFooterTemplate>
                    <span>Total:</span>
                </ng-template>
            </kendo-grid-column>
            
            <kendo-grid-column field="MultiDebit" *ngIf="isMultiCurrency == true"  
                title="Debit" 
                [headerClass]="{'text-right':true}"
                [class]="{'text-right':true}" 
                [footerClass]="{'text-right':true}" 
                width="120">
                <ng-template kendoGridCellTemplate let-dataItem>
                    <span> {{dataItem['MultiDebit'] | currency }}</span>
                </ng-template>
                <ng-template kendoGridFooterTemplate>
                    <span>{{multiTotalDebit | async | currency }}</span>
                </ng-template>
            </kendo-grid-column>
            
            <kendo-grid-column field="MulitCredit" *ngIf="isMultiCurrency == true" title="Credit" 
                [headerClass]="{'text-right':true}" 
                [footerClass]="{'text-right':true}"
                [class]="{'text-right':true}" 
                width="120">
                <ng-template kendoGridCellTemplate let-dataItem>
                    <span>{{dataItem['MulitCredit'] | currency }}</span>
                </ng-template>
                <ng-template kendoGridFooterTemplate>
                    <span>{{multiTotalCredit | async | currency }}</span>
                </ng-template>
            </kendo-grid-column>

            <kendo-grid-column field="BaseDebit"  [title]="isMultiCurrency == true ? 'Debit (In Base)': 'Debit'" 
                [headerClass]="{'text-right':true}"
                [class]="{'text-right':true}" 
                [footerClass]="{'text-right':true}" 
                width="120">
                <ng-template kendoGridCellTemplate let-dataItem>
                    <span> {{dataItem['BaseDebit'] | currency }}</span>
                </ng-template>
                <ng-template kendoGridFooterTemplate>
                    <span>{{baseTotalDebit | async | currency }}</span>
                </ng-template>
            </kendo-grid-column>

            <kendo-grid-column field="BaseCredit"  [title]="isMultiCurrency == true ? 'Credit (In Base)': 'Credit'" 
                [headerClass]="{'text-right':true}"
                [class]="{'text-right':true}" 
                [footerClass]="{'text-right':true}" 
                width="120">
                <ng-template kendoGridCellTemplate let-dataItem>
                    <span> {{dataItem['BaseCredit'] | currency }}</span>
                </ng-template>
                <ng-template kendoGridFooterTemplate>
                    <span>{{baseTotalCredit | async | currency }}</span>
                </ng-template>
            </kendo-grid-column>

        </kendo-grid>
    </div>
    `,
    providers: [
        AccountSetupService,
        RecapGridSubject
    ],
    styles: [`

        .k-grid-footer{
            background-color:#DDD;
        }

        tr.text-center {
            text-align:center
        }

        tr.text-right {
            text-align:right
        }
    `]
})
export class RecapDailogGridComponent implements OnInit, OnDestroy {

    @ViewChild(PGridViewComponent, { static: true }) private _gridView: PGridViewComponent;
    @Input() fetch: Observable<any>;

    public multiTotalDebit = new BehaviorSubject<number>(0);
    public multiTotalCredit = new BehaviorSubject<number>(0);
    public baseTotalDebit = new BehaviorSubject<number>(0);
    public baseTotalCredit = new BehaviorSubject<number>(0);
    public isMultiCurrency = false;

    private _subsink = new SubSink()

    constructor(private _accountSetupService: AccountSetupService,
        public view: RecapGridSubject) {
    }

    ngOnInit(): void {

        this.view.loading = true;
        this._subsink.sink = this.fetch.pipe(
            map((response: any[]) => {
                response.map(m => {
                    if (m['CURRSK'] == environment.general.currSK) {
                        m['Multi'] = false;
                    } else {
                        m['Multi'] = true;
                    }
                });

                return response;
            }),
            map((response: any[]) => {
                response.map(m => {

                    if (m['Multi'] == true) {
                        if (m.TransactionAmount > 0) {
                            m['MultiDebit'] = multiCurrency({
                                value: +parseFloat(m.TransactionAmount).toFixed(2),
                                op: m.CURROP,
                                rate: m.CURRATE
                            })
                            m['MulitCredit'] = 0;
                        } else {
                            m['MultiDebit'] = 0;
                            let value = (parseFloat(m.TransactionAmount) * (-1)).toFixed(2);
                            m['MulitCredit'] = multiCurrency({
                                value: +value,
                                op: m.CURROP,
                                rate: m.CURRATE
                            });
                        }
                    }

                    if (m.TransactionAmount > 0) {
                        m['BaseDebit'] = m.TransactionAmount;
                        m['BaseCredit'] = 0;
                    } else {
                        m['BaseDebit'] = 0;
                        m['BaseCredit'] = m.TransactionAmount * (-1);
                    }
                })

                return response;
            }),
            mergeMap((response: any[]) => {
                return this._accountSetupService.finds({
                    filter: {
                        logic: 'and',
                        filters: kendoFilter([
                            { field: 'AccountType', value: 1, operator: 'eq', logic: 'and' }
                        ])
                    }
                }).pipe(
                    map((d: any[]) => {

                        response.map(m => {

                            const acc = d.find(x => x.SysKey == m['AccSK']);

                            if (typeof acc == 'undefined')
                                alert(`${m['AccSK']} Some account code is missing.`)

                            if (typeof acc != 'undefined') {
                                m['AccountCode'] = acc['AccCode'];
                                m['Account'] = acc['AccDescription'];
                            }
                        });

                        return response;
                    })
                )
            }),
            finalize(() => {
                this.view.loading = false;
            })
        ).subscribe((response: any[]) => {

            this.view.next(response);

            let total = cloneData(response);
            this.isMultiCurrency = response.filter(x => x.Multi == true).length > 0;
            this.multiTotalDebit.next(total.reduce((sum, current) => sum + current['MultiDebit'], 0));
            this.multiTotalCredit.next(total.reduce((sum, current) => sum + current['MulitCredit'], 0));
            this.baseTotalDebit.next(total.reduce((sum, current) => sum + current['BaseDebit'], 0));
            this.baseTotalCredit.next(total.reduce((sum, current) => sum + current['BaseCredit'], 0));
        });
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }
}

