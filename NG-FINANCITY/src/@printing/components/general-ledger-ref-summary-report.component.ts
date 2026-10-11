import { Component, OnInit, ViewChild, NgModule } from '@angular/core';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { kendoFilter } from 'src/@common/service/utility-service';
import { GeneralLedgerRefSummaryReportService } from '../services/general-ledger-ref-summary.report.service';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@Component({
    selector: 'app-general-ledger-ref-summary-report',
    template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='General Ledger Ref. Summary' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterItemTraversalService,
        GeneralLedgerRefSummaryReportService,
        AccountSetupService,
    ]
})
export class GeneralLedgerRefSummaryReportComponent extends DetailFormBase implements OnInit {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _accountSetupService: AccountSetupService,
        private _reportService: GeneralLedgerRefSummaryReportService,
        private _filterItemService: FilterItemService,
        private _traversalService: FilterItemTraversalService) {

        super(fb);
    }

    ngOnInit() {

        this._toolbar
            .controls
            .Success(this.OnSuccess)
            .Ready(this.formReady)
            .StateKey(['RecordStatus'])
            .NavigationButton({ action: () => this._router.navigateByUrl('') })
            .ExportToPdf({
                action: () => {
                    let f = this._traversalService.traverseFilter(this.filter.form.get('formItems').value);
                    return this._reportService.pdf({
                        filter: {
                            filters: f,
                            logic: 'and'
                        }
                    })
                }
            })

        this.form = this.fb.group({ RecordStatus: 2 });
        this.filter.type = 2;

        this._filterItemService.translateForm({
            columns: [{
                dataType: 'string', field: 'AccountSK', filter: true, title: 'Account Code',
                itemTemplateField: {
                    field1: 'AccCode',
                    field2: 'AccDescription',
                },
                filterOptions: {
                    control: 'dropdown',
                    source: this._accountSetupService,
                    valueField: 'SysKey',
                    textField: 'AccCode',
                    filterChange: (value) => {
                        if (value.length >= 3) {
                            this._accountSetupService.asyncFinds({
                                filter: {
                                    logic: "and",
                                    filters: kendoFilter([
                                        { field: 'AccCode', value: `<# ${value}`, operator: 'contains', logic: 'or' },
                                        { field: 'AccDescription', value: `${value} #>`, operator: 'contains', logic: 'and' },
                                        { field: 'AccountType', value: 1, operator: 'eq', logic: 'and' },
                                    ]),
                                }
                            })
                        }
                    }
                }
            }, {
                dataType: 'string', field: 'SalesDate', filter: true, title: 'Account Post Date',
                filterOptions: {
                    control: 'date-period',
                }
            }]
        });

        this.filter.form = this._filterItemService.form;
    }
}

@NgModule({
    declarations: [GeneralLedgerRefSummaryReportComponent],
    imports: [
        CommonModule,
        PToolbarModule,
        FilterPanelModule,
        InputsModule,
        PInputIconDirectiveModule,
        RouterModule.forChild([{ path: '', component: GeneralLedgerRefSummaryReportComponent }])
    ],
    exports: [GeneralLedgerRefSummaryReportComponent]
})
export class Module { }