import { Component, OnInit, ViewChild, NgModule } from '@angular/core';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { CashbookReceiptReportService } from '../services/cashbook-receipt-report.service';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@Component({
    selector: 'app-cashbook-receipt-log-report',
    template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='Cashbook Receipt Log' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterComponent,
        CashbookReceiptReportService,
        CashbookSetupService,
        FilterItemTraversalService
    ]
})
export class CashbookReceiptLogReportComponent extends DetailFormBase implements OnInit {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _reportService: CashbookReceiptReportService,
        private _filterItemService: FilterItemService,
        private _traversalService: FilterItemTraversalService,
        private _cashbookService: CashbookSetupService) {

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
                },
                validate: () => {
                    this._traversalService.markFormTouch(this.filter.form.get('formItems').value);
                    return this._traversalService.isValid;
                }
            })

        this.form = this.fb.group({ RecordStatus: 2 });
        this.filter.type = 2;

        this._filterItemService.translateForm({
            columns: [{
                dataType: 'string', field: 'CBSK', filter: true, title: 'Cashbook', validators: [Validators.required],
                filterOptions: {
                    control: 'dropdown',
                    source: this._cashbookService,
                    valueField: 'SysKey',
                    textField: 'CashbookName',
                }
            }, {
                dataType: 'string', field: 'SalesDate', filter: true, title: 'Account Post Date',
                filterOptions: {
                    control: 'date-period',
                }
            }]
        });

        this._cashbookService.asyncFinds();
        this.filter.form = this._filterItemService.form;
    }
}

@NgModule({
    declarations: [CashbookReceiptLogReportComponent],
    imports: [
        CommonModule,
        PToolbarModule,
        FilterPanelModule,
        InputsModule,
        PInputIconDirectiveModule,
        RouterModule.forChild([{ path: '', component: CashbookReceiptLogReportComponent }])
    ],
    exports: [CashbookReceiptLogReportComponent]
})
export class Module { }