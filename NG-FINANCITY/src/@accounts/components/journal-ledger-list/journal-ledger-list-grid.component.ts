import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { JournalLedgerService } from 'src/@accounts/services/journal-ledger-service';
import { Router } from '@angular/router';
import { accountingModules, recordStatusOptions } from 'src/@common/service/utility-service';
import { JournalLedgerGridDetailComponent } from './journal-ledger-list-grid.-detail.component';

@Component({
    selector: 'app-journal-ledger-grid',
    template: `
        <p-grid-view>
            <ng-template let-dataItem="dataItem">
               <app-journal-ledger-grid-detail [parentId]="dataItem.ParentId"
               [postDate]="dataItem.PostDate"
               [moduleCode]="dataItem.ModuleCode"
               [yc]="dataItem.YC"
               [canDetail]="canDetail"
               [status]="dataItem.RecordStatus"
               ></app-journal-ledger-grid-detail>
            </ng-template>
        </p-grid-view>
    `,
    providers: [
        JournalLedgerService,
    ]
})
export class JournalLedgerGridComponent implements OnInit {

    @Input() moduleCode: string;
    @Input() accountingYear: number;
    @Input() canDetail: boolean;

    @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

    modules = accountingModules();

    constructor(private _journalLedger: JournalLedgerService,
        private _router: Router) {
    }

    ngOnInit() {
        this._gridView.state.take = 13;
        this.intializeGridSource();
    }

    refresh() {
        this._gridView.config = {
            key: 'ParentId',
            methodName: 'getPager',
            fetch: this._journalLedger,
            filterable: 'window',
            detailTemplate: JournalLedgerGridDetailComponent,
            columns: [
                { dataType: 'hidden', field: 'YC', filter: false, title: '' },
                { dataType: 'string', field: 'R1', filter: true, width: 120, title: 'Trans. No.', align: 'center' },
                {
                    dataType: 'string', field: 'ModuleCode', filter: true, width: 120, title: 'Module', align: 'center',
                    filterOptions: {
                        source: this.modules, control: "multi-select", operator: 'eq'
                    }
                },
                {
                    dataType: 'date', field: 'PostDate', filter: true, width: 120, title: 'Post Date', filterOptions: {
                        operator: 'bwt', defaultValue1: `${this.accountingYear}0101`, defaultValue2: `${this.accountingYear}1231`
                    }
                },
                { dataType: 'string', field: 'SecondRef', filter: true, width: 120, title: 'Second Ref.' },
                { dataType: 'string', field: 'Description', filter: true, title: 'Description' },
                { dataType: 'currency', field: 'TotalDebit', filter: false, width: 120, title: 'Total Debit' },
                { dataType: 'currency', field: 'TotalCredit', filter: false, width: 120, title: 'Total Credit' },
                {
                    dataType: 'record_status', field: 'RecordStatus', filter: true, title: 'Status', filterOptions: {
                        source: recordStatusOptions().filter(x => x.value != -1 && x.value != 1),
                        defaultValue1: [2, 8, 125, 126],
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


