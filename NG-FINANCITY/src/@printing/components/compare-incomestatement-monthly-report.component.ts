import { Component, OnInit, OnDestroy, ViewChild, NgModule } from '@angular/core';
import { from } from 'rxjs';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { map, groupBy } from 'rxjs/operators';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { AccountPeriodService } from 'src/@accounts/services/account-period.service';
import { CompareIncomeStatementMonthlyReportService } from '../services/compare-incomestatement-monthly-report.service';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
    selector: 'app-incomestatement-monthly-report',
    template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='Comparable Income Statement [Monthly]' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
    providers: [
        FilterItemService,
        FilterItemTraversalService,
        CompareIncomeStatementMonthlyReportService,
        AccountPeriodService,
    ]
})
export class CompareIncomeStatementMonthlyReportComponent extends DetailFormBase implements OnInit, OnDestroy {

    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild('filter', { static: true }) filter: FilterComponent;

    private _subsink = new SubSink();

    constructor(fb: FormBuilder,
        private _router: Router,
        private _accountingPeriodService: AccountPeriodService,
        private _reportService: CompareIncomeStatementMonthlyReportService,
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
                dataType: 'string', field: 'YCStart', filter: true, title: 'Accounting Year', validators: [Validators.required],
                filterOptions: {
                    control: 'dropdown',
                    source: this._accountingPeriodService,
                    valueField: 'value',
                    textField: 'text',
                }
            }]
        });

        this._subsink.sink = this._accountingPeriodService.finds()
            .pipe(
                map(data => data.sort(this.compareFn)),
                map((data: { YC: string, Date1: number }[]) => data),
            )
            .subscribe(data => {

                this._subsink.sink = from(data)
                    .pipe(
                        map(({ YC }) => YC),
                        groupBy(group => group),
                    ).subscribe(g => {
                        let result = [];
                        result.push({
                            value: g.key,
                            text: `${g.key} [${data.filter(x => x.YC == g.key)[0].Date1.toString().substring(0, 4)}]`
                        })
                        this._accountingPeriodService.next(result);
                    })
            })

        this.filter.form = this._filterItemService.form;
    }

    private compareFn = (a, b) => {

        if (a.YC < b.YC) return -1;
        if (a.YC > b.YC) return 1;
        return 0;
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }
}

@NgModule({
    declarations: [CompareIncomeStatementMonthlyReportComponent],
    imports: [
        CommonModule,
        PToolbarModule,
        FilterPanelModule,
        InputsModule,
        PInputIconDirectiveModule,
        RouterModule.forChild([{ path: '', component: CompareIncomeStatementMonthlyReportComponent }])
    ],
    exports: [CompareIncomeStatementMonthlyReportComponent]
})
export class Module { }