
import { Component, OnInit, ViewChild, NgModule } from '@angular/core';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { IncomeStatementReportService } from '../services/income-statement-report.service';
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
                <p-toolbar title='Income Statement' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterItemTraversalService,
        IncomeStatementReportService
    ]
})
export class IncomeStatementComponent extends DetailFormBase implements OnInit {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    constructor(fb: FormBuilder,
        private _router: Router,
        private _incomeStatementReport: IncomeStatementReportService,
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
                    return this._incomeStatementReport.pdf({
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
                dataType: 'string', field: 'AccountingDate', filter: true, title: 'Account Post Date',
                filterOptions: {
                    control: 'date-period',
                }
            }]
        });

        this.filter.form = this._filterItemService.form;
    }
}

@NgModule({
    declarations: [IncomeStatementComponent],
    imports: [
        CommonModule,
        PToolbarModule,
        FilterPanelModule,
        InputsModule,
        PInputIconDirectiveModule,
        RouterModule.forChild([{ path: '', component: IncomeStatementComponent }])
    ],
    exports: [IncomeStatementComponent]
  })
  export class Module { }