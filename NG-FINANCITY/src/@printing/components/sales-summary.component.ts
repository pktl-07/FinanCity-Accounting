import { Component, OnInit, ViewChild, NgModule } from '@angular/core';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { SalesSummaryReportSerivce } from '../services/sales-summary-report.service';
import { TNC_PropertyTypeService } from 'src/@sales/services/tnc_propertyType.service';
import { TNC_TransactionTypeService } from 'src/@sales/services/tnc_transactionType.serivce';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@Component({
    selector: 'app-income-statement',
    template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='Sales Summary' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterItemTraversalService,
        SalesSummaryReportSerivce,
        TNC_PropertyTypeService,
        TNC_TransactionTypeService,
    ]
})
export class SalesSummaryComponent extends DetailFormBase implements OnInit {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _salesSummaryReportSerivce: SalesSummaryReportSerivce,
        private _filterItemService: FilterItemService,
        private _tncPropertyTypeService: TNC_PropertyTypeService,
        private _tncTransactionTypeService: TNC_TransactionTypeService,
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
                    return this._salesSummaryReportSerivce.pdf({
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
                dataType: 'sales-status', field: 'ReceivedStatus', filter: true, title: 'Commission Receivable Status',
                filterOptions: {
                    defaultValue1: [2],
                    control: 'multi-select',
                }
            }, {
                dataType: 'string', field: 'TransactionType', filter: true, title: 'Transaction Type',
                filterOptions: {
                    control: 'dropdown',
                    source: this._tncTransactionTypeService,
                    valueField: 'SysKey',
                    textField: 'Description',
                }
            }, {
                dataType: 'string', field: 'PropertyType', filter: true, title: 'Property Type',
                filterOptions: {
                    control: 'dropdown',
                    source: this._tncPropertyTypeService,
                    valueField: 'SysKey',
                    textField: 'Description',
                }
            }, {
                dataType: 'string', field: 'SalesDate', filter: true, title: 'Invoice Date',
                filterOptions: {
                    control: 'date-period',
                }
            }]
        });

        this._tncTransactionTypeService.asyncFinds();
        this._tncPropertyTypeService.asyncFinds();

        this.filter.form = this._filterItemService.form;
    }
}

@NgModule({
    declarations: [SalesSummaryComponent],
    imports: [
        CommonModule,
        PToolbarModule,
        FilterPanelModule,
        InputsModule,
        PInputIconDirectiveModule,
        RouterModule.forChild([{ path: '', component: SalesSummaryComponent }])
    ],
    exports: [SalesSummaryComponent]
})
export class Module { }