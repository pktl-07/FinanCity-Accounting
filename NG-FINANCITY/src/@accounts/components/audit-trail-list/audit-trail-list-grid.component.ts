import { Component, OnInit, Input, ViewChild, OnDestroy } from '@angular/core';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { JournalLedgerService } from 'src/@accounts/services/journal-ledger-service';
import { accountingModules, recordStatusOptions, SubSink } from 'src/@common/service/utility-service';
import { AuditTrailGridDetailComponent } from './audit-trail-list-grid-detail.component';

@Component({
    selector: 'app-audit-trail-grid',
    template: `
        <p-grid-view>
            <ng-template let-dataItem="dataItem">
               <app-audit-trail-grid-detail [parentId]="dataItem.ParentId"
               [postDate]="dataItem.PostDate"
               [moduleCode]="dataItem.ModuleCode"
               [yc]="dataItem.YC"
               [canDetail]="canDetail"
               [status]="dataItem.RecordStatus"
               ></app-audit-trail-grid-detail>
            </ng-template>
        </p-grid-view>
    `,
    providers: [
        JournalLedgerService,
    ]
})
export class AuditTrailListGridComponent implements OnInit , OnDestroy {

    @Input() moduleCode: string;
    @Input() accountingYear: number;
    @Input() canDetail: boolean;

    @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

    modules = accountingModules();

    private _subsink = new SubSink();

    constructor(private _journalLedger: JournalLedgerService) {
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    ngOnInit() {
        this._gridView.state.take = 13;
        this.intializeGridSource();
    }

    refresh() {
        this._gridView.config = {
            key: 'ParentId',
            methodName: 'getAuditPager',
            fetch: this._journalLedger,
            filterable: 'window',
            detailTemplate: AuditTrailGridDetailComponent,
            columns: [
                { dataType: 'hidden', field: 'YC', filter: false, title: '' },
                { dataType: 'string', field: 'R1', filter: true, width: 90, title: 'Trans. No.', align: 'center' },
                {
                    dataType: 'string', field: 'ModuleCode', filter: true, width: 60, title: 'Module', align: 'center',
                    filterOptions: {
                        source: this.modules, control: "multi-select", operator: 'eq'
                    }
                },
                {
                    dataType: 'date', field: 'PostDate', filter: true, width: 100, title: 'Post Date', filterOptions: {
                        operator: 'bwt', defaultValue1: `${this.accountingYear}0101`, defaultValue2: `${this.accountingYear}1231`
                    }
                },
                {
                    dataType: 'string', field: 'LoginUser', filter: true, width: 120, title: 'Created By'
                },
                {
                    dataType: 'date', field: 'CreatedDate', filter: true, width: 100, title: 'Created Date'
                },
                {
                    dataType: 'string', field: 'CreatedTime', filter: false, width: 100, title: 'Created Time'
                },
                { dataType: 'string', field: 'SecondRef', filter: true, title: 'Second Ref.' },
                { dataType: 'string', field: 'Description', filter: true, width: 200, title: 'Description' },
                { dataType: 'currency', field: 'TotalDebit', filter: true, width: 120, title: 'Total Debit' },
                { dataType: 'currency', field: 'TotalCredit', filter: true, width: 120, title: 'Total Credit' },
                {
                    dataType: 'record_status', field: 'RecordStatus', filter: true, title: 'Status', filterOptions: {
                        source: recordStatusOptions().filter(x => x.value != -1 && x.value != 1),
                        defaultValue1: [6, 8, 125, 126],
                    },
                },
            ]
        }

        let m = this._gridView.config.columns.find(x => x.field == 'ModuleCode');
        if (this.moduleCode) {
            m.filterOptions.defaultValue1 = [this.moduleCode]
        }
    }

    private intializeGridSource() {
        this.refresh();
    }
}


