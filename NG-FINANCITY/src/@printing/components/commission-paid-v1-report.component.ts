import { Component, OnInit, ViewChild } from '@angular/core';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { kendoFilter } from 'src/@common/service/utility-service';
import { CommissionPayableBSReportService } from '../services/commission-payable-bs-report.service';
import { CommissionPiadV1ReportService } from '../services/commission-paid-v1-report.service';

@Component({
    selector: 'app-commisison-paid-v1-report',
    template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='Commission Paid (By Individual SP - IRAS)' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterItemTraversalService,
        CommissionPayableBSReportService,
        SalespersonManagementService,
        CommissionPiadV1ReportService
    ]
})
export class CommissionPiadV1ReportComponent extends DetailFormBase implements OnInit {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _salespersonManagementService: SalespersonManagementService,
        private _service: CommissionPiadV1ReportService,
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
                    return this._service.pdf({
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
                    dataType: 'string', field: 'SalespersonType', filter: true, title: 'Salesperson Type',
                    valueChange: (item => {
                        if (item != null) {
                            this._salespersonManagementService.asyncEveryBody({
                                filter: {
                                    logic: 'and',
                                    filters: kendoFilter([
                                        { field: 'TransType', value: item, logic: 'and', operator: 'eq' }
                                    ])
                                }
                            })
                        } else {
                            this._salespersonManagementService.next([]);
                        }
                    }),
                    filterOptions: {
                        control: 'dropdown',
                        source: [{ value: 25, text: 'Internal Salesperson' }, { value: 20, text: 'External Salesperson' }],
                        defaultValue1: 25,
                        valueField: 'value',
                        textField: 'text',
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