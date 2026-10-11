import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { AccountPeriodService } from 'src/@accounts/services/account-period.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { BehaviorSubject } from 'rxjs';
import { finalize, mergeMap, map } from 'rxjs/operators';

@Component({
    selector: 'app-chart-of-accounts-grid',
    template: `
    <kendo-grid [style.height.%]="100" [data]="view | async" [resizable]="true" [loading]="loading">
        <kendo-grid-column title="Account Code" width="80">
            <ng-template kendoGridCellTemplate let-dataItem>
                <span [style.marginLeft.px]="getMargin(dataItem.Index)" 
                [ngClass]="{'text-bold': dataItem.AccountType == 2}">
                    {{ dataItem.AccountCode }}
                </span>   
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Account Name" width="200">
            <ng-template kendoGridCellTemplate let-dataItem>
                <span [style.marginLeft.px]="getMargin(dataItem.Index)" 
                [ngClass]="{'text-bold': dataItem.AccountType == 2}">
                    {{ dataItem.AccountName }}
                </span>   
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Type" width="120">
            <ng-template kendoGridCellTemplate let-dataItem>
                <span [style.marginLeft.px]="getMargin(dataItem.Index)" 
                [ngClass]="{'text-bold': dataItem.AccountType == 2}">
                    {{ dataItem.CategoryDescription }}
                </span>   
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Default" width="120">
            <ng-template kendoGridCellTemplate let-dataItem>
                {{ dataItem.ControlAccountDescription }}{{ dataItem.ControlAccountDescription ? '*' : '' }}
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Debit" width="80" [headerClass]="{'text-right':true}"  [class]="{'text-right':true}">
            <ng-template kendoGridCellTemplate let-dataItem>
                <span [ngClass]="{'text-bold': dataItem.AccountType == 2}">
                    {{ dataItem.Debit | currency }}
                </span>   
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Credit" width="80" [headerClass]="{'text-right':true}" [class]="{'text-right':true}">
            <ng-template kendoGridCellTemplate let-dataItem>
                <span [ngClass]="{'text-bold': dataItem.AccountType == 2}">
                    {{ dataItem.Credit | currency }}
                </span>   
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column width="60">
            <ng-template kendoGridCellTemplate let-dataItem>
                <button *ngIf="dataItem.AccountCode.indexOf('0000') == -1"
                class="form-control btn-primary" (click)='handleDetail(dataItem)'>Detail</button>
            </ng-template>
        </kendo-grid-column>
    </kendo-grid>
    `,
    providers: [
        AccountSetupService,
        AccountPeriodService,
    ],
    styles: [`

        tr span .text-right {
            text-align:right;
        }

      .text-bold {
        font-weight: bold;
      }
   `]
})
export class ChartOfAccountGridComponent implements OnInit, OnDestroy {

    @Input() id: string;
    @Input() accountPrefix: string;
    @Output() clickDetail = new EventEmitter<any>();

    view = new BehaviorSubject<any>([]);
    loading: boolean;
    private _subsink = new SubSink();

    constructor(private _accountSetupService: AccountSetupService,
        private _accountingPeriod: AccountPeriodService) {
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    ngOnInit() {
        this.intializeGridSource();
    }

    refresh() {

        this.loading = true;
        this._subsink.sink = this._accountingPeriod.finds({
            filter: {
                logic: 'and',
                filters: kendoFilter([
                    { field: 'IsDefaultYear', value: true, operator: 'eq', logic: 'and' }
                ])
            }
        }).pipe(mergeMap(response => {
            const yc = response[0]['YC'];
            return this._accountSetupService
                .getChartOfAccount(yc, this.accountPrefix)
                .pipe(map(data => data))
        }),
            finalize(() => this.loading = false))
            .subscribe((data) => {
                this.view.next(data);
            })
    }

    getMargin(value: number) {
        return value * 15;
    }

    handleDetail(dataItem: any) {

        this.clickDetail.emit(dataItem);
    }

    private intializeGridSource() {
        this.refresh();
    }
}


