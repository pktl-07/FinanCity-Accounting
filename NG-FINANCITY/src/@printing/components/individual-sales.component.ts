import { Component, OnInit, ViewChild, NgModule } from '@angular/core';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { IndividualSalesReportService } from '../services/individual-sales-report.service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { kendoFilter } from 'src/@common/service/utility-service';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@Component({
    selector: 'app-individual-sales',
    template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='Individual Sales' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterItemTraversalService,
        IndividualSalesReportService,
        SalespersonManagementService,
    ]
})
export class IndividualSalesComponent extends DetailFormBase implements OnInit {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _salespersonManagementService: SalespersonManagementService,
        private _individualSalesRport: IndividualSalesReportService,
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
                    return this._individualSalesRport.pdf({
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
                }
            }, {
                dataType: 'string', field: 'SPSK', filter: true, title: 'Salesperson',
                filterOptions: {
                    control: 'dropdown',
                    source: this._salespersonManagementService,
                    filterChange: (text => {
                        this._salespersonManagementService.asyncFinds({
                            filter: {
                                logic: 'or',
                                filters: kendoFilter([{
                                    field: 'Name', value: text, logic: 'or', operator: 'contains'
                                }])
                            }
                        });
                    }),
                    valueField: 'SysKey',
                    textField: 'Name',
                }
            }, {
                dataType: 'string', field: 'SalesDate', filter: true, title: 'Invoice Date',
                filterOptions: {
                    control: 'date-period',
                }
            }]
        });

        this.filter.form = this._filterItemService.form;
    }
}

@NgModule({
    declarations: [IndividualSalesComponent],
    imports: [
        CommonModule,
        PToolbarModule,
        InputsModule,
        FilterPanelModule,
        PInputIconDirectiveModule,
        RouterModule.forChild([{ path: '', component: IndividualSalesComponent }])
    ],
    exports: [IndividualSalesComponent]
})
export class Module { }