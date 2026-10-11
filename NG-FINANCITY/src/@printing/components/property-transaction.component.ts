import { Component, OnInit, ViewChild, NgModule } from '@angular/core';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { PropertyTransactionReportService } from '../services/property-transaction-report.service';
import { recordStatusOptions, kendoFilter } from 'src/@common/service/utility-service';
import { TNC_PropertyTypeService } from 'src/@sales/services/tnc_propertyType.service';
import { TNC_TransactionTypeService } from 'src/@sales/services/tnc_transactionType.serivce';
import { VendorService } from 'src/@sales/services/vendor.service';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@Component({
    selector: 'app-sales-detail',
    template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='Sales Transaction Order List' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterItemTraversalService,
        PropertyTransactionReportService,
        TNC_PropertyTypeService,
        TNC_TransactionTypeService,
        VendorService,
    ]
})
export class PropertyTransactionComponent extends DetailFormBase implements OnInit {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _vendorService: VendorService,
        private _transactionTypeService: TNC_TransactionTypeService,
        private _propertyTransactionReportService: PropertyTransactionReportService,
        private _propertyTypeService: TNC_PropertyTypeService,
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
                    return this._propertyTransactionReportService.pdf({
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
            columns: [
                {
                    dataType: 'record_status', field: 'RecordStatus', filter: true, title: 'Sales Transaction Status', filterOptions: {
                        source: recordStatusOptions().filter(x => x.value == 1 || x.value == 2 || x.value == 3 || x.value == 8),
                        defaultValue1: [8],
                    }
                },
                {
                    dataType: 'string', field: 'TransactionType', filter: true, title: 'Transaction Type',
                    filterOptions: {
                        control: 'dropdown',
                        source: this._transactionTypeService,
                        valueField: 'SysKey',
                        textField: 'Description',
                    }
                },
                {
                    dataType: 'string', field: 'PropertyType', filter: true, title: 'Property Type',
                    filterOptions: {
                        control: 'dropdown',
                        source: this._propertyTypeService,
                        valueField: 'SysKey',
                        textField: 'Description',
                    }
                },
                {
                    dataType: 'string', field: 'VendorSK', filter: true, title: 'Bank',
                    filterOptions: {
                        control: 'dropdown',
                        source: this._vendorService,
                        valueField: 'SysKey',
                        textField: 'CompanyName',
                    }
                },
                {
                    dataType: 'string', field: 'SalesDate', filter: true, title: 'Sales Transaction Date',
                    filterOptions: {
                        control: 'date-period',
                    }
                }]
        });

        this._transactionTypeService.asyncFinds();
        this._propertyTypeService.asyncFinds();
        this._vendorService.asyncFinds({
            filter: {
                logic: 'and',
                filters: kendoFilter([
                    { field: 'SysKey', value: '1', logic: 'and', operator: 'neq' }
                ])
            }
        });

        this.filter.form = this._filterItemService.form;
    }
}

@NgModule({
    declarations: [PropertyTransactionComponent],
    imports: [
        CommonModule,
        PToolbarModule,
        FilterPanelModule,
        InputsModule,
        PInputIconDirectiveModule,
        RouterModule.forChild([{ path: '', component: PropertyTransactionComponent }])
    ],
    exports: [PropertyTransactionComponent]
})
export class Module { }