import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { BehaviorSubject, combineLatest } from 'rxjs';
import { JournalLedgerService } from 'src/@accounts/services/journal-ledger-service';
import { Router } from '@angular/router';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { mergeMap, map, finalize } from 'rxjs/operators';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { TaxService } from 'src/@accounts/services/tax.service';

@Component({
    selector: 'app-journal-ledger-grid-detail',
    template: `
    <kendo-grid [data]="view | async" [resizable]="true" [loading]="loading">
        <kendo-grid-column *ngIf="canDetail" [width]="40">
            <ng-template kendoGridCellTemplate let-dataItem>
                <a style="cursor: pointer" (click)="handleDetail()">
                    <i style="color:var(--primary)" class="fa fa-th"></i>
                </a>
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Account Code" width="120" [headerClass]="{'text-center':true}"  [class]="{'text-center':true}">
            <ng-template kendoGridCellTemplate let-dataItem>
                {{ dataItem.AccCode }}
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Account Name" width="200">
            <ng-template kendoGridCellTemplate let-dataItem>
                {{ dataItem.AccDescription }}
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Debit" width="120" [headerClass]="{'text-right':true}"  [class]="{'text-right':true}">
            <ng-template kendoGridCellTemplate let-dataItem>
                {{ dataItem.Debit | currency }}
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Credit" width="120" [headerClass]="{'text-right':true}"  [class]="{'text-right':true}">
            <ng-template kendoGridCellTemplate let-dataItem>
               {{ dataItem.Credit | currency }}
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="GST" width="120" [headerClass]="{'text-center':true}"  [class]="{'text-center':true}">
            <ng-template kendoGridCellTemplate let-dataItem>
            {{ dataItem.TaxCode }}
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="GST (%)" width="120" [headerClass]="{'text-center':true}"  [class]="{'text-center':true}">
            <ng-template kendoGridCellTemplate let-dataItem>
            {{ dataItem.TaxPercent }}
            </ng-template>
        </kendo-grid-column>
        <kendo-grid-column title="Memo">
            <ng-template kendoGridCellTemplate let-dataItem>
               {{ dataItem.LineDescription }}
            </ng-template>
        </kendo-grid-column>
    </kendo-grid>
    `,
    providers: [
        JournalLedgerService,
        AccountSetupService,
        TaxService,
    ],
    styles: [`

        tr span .text-right {
            text-align:right;
        }

        tr span .text-center {
            text-align:center;
        }

      .text-bold {
        font-weight: bold;
      }
   `]
})
export class JournalLedgerGridDetailComponent implements OnInit, OnDestroy {

    @Input() parentId: string;
    @Input() postDate: string;
    @Input() moduleCode: string;
    @Input() yc: string;
    @Input() canDetail: boolean;
    @Input() status: number;

    view = new BehaviorSubject<any>([]);
    loading: boolean;
    private _subsink = new SubSink();

    constructor(private _journalLedger: JournalLedgerService,
        private _router: Router,
        private _accountSetupService: AccountSetupService,
        private _taxService: TaxService,
    ) { }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    ngOnInit() {

        this.loading = true;
        this._subsink.sink = this._journalLedger.finds({
            filter: {
                filters: kendoFilter([
                    { field: 'ParentId', value: this.parentId, logic: 'and', operator: 'eq' },
                    { field: 'ModuleCode', value: this.moduleCode, logic: 'and', operator: 'eq' },
                    { field: 'PostDate', value: this.postDate, logic: 'and', operator: 'eq' },
                    { field: 'RecordStatus', value: this.status, logic: 'and', operator: 'eq' },
                    { field: 'YCPC', value: this.yc, logic: 'and', operator: 'startswith' },
                ]),
                logic: 'and'
            },
        }).pipe(map((response: any[]) => {
            response.map(d => {
                if (d.TransactionAmount > 0) {
                    d['Debit'] = d.TransactionAmount;
                    d['Credit'] = 0;
                } else {
                    d['Debit'] = 0;
                    d['Credit'] = d.TransactionAmount * (-1);
                }
            })
            return response;
        }),
            mergeMap((response: any[]) => {
                let s = new Array<any>();
                response.map(m => {
                    const d = this._accountSetupService.findByKey(m['AccSK'])
                        .pipe(map((e) => {
                            m['AccCode'] = e['AccCode'];
                            m['AccDescription'] = e['AccDescription'];
                            return m;
                        }));
                    s.push(d);
                })
                return combineLatest(s);
            }),
            mergeMap((response: any[]) => {
                let s = new Array<any>();
                response.map(m => {
                    const d = this._taxService.findByKey(m['TaxSK'])
                        .pipe(map((e) => {

                            if (e['TaxCode'] == null)
                                m['TaxCode'] = 'N/A';
                            else
                                m['TaxCode'] = e['TaxCode'];
                            return m;
                        }));
                    s.push(d);
                })
                return combineLatest(s);
            }), map((data: any[]) => {

                if (this.moduleCode == 'GLC') {
                    data.map((m, i) => {

                        if (m['HasTaxLine'] == true) {
                            let taxLine = data[i + 1];
                            m['TaxCode'] = taxLine['TaxCode'];
                            m['TaxPercent'] = taxLine['TaxPercent'];

                            taxLine['TaxCode'] = 'N/A';
                            taxLine['TaxPercent'] = 0;
                        }
                        return data;
                    });
                }
                return data;
            }),
            finalize(() => this.loading = false)
        ).subscribe((data) => {
            this.view.next(data);
        })
    }

    handleDetail() {

        this._router.navigate([`Accounting/JournalLedgerDetail/${this.parentId}/${this.moduleCode}/${this.postDate}`]);
    }
}


