import { Component, OnInit, OnDestroy, ViewChild, NgModule } from '@angular/core';
import { from } from 'rxjs';
import { FilterComponent } from 'src/@common/components/p-filter-panel/p-filter-panel-component';
import { FilterItemService } from 'src/@common/components/p-filter-panel/p-filter-item-service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AccountPeriodService } from 'src/@accounts/services/account-period.service';
import { map, groupBy } from 'rxjs/operators';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { FilterItemTraversalService } from 'src/@common/components/p-filter-panel/p-filter-item-traverse-service';
import { TrialBalanceReportService } from '../services/trial-balance-report.service';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@Component({
  selector: 'app-trial-balance',
  template: `
        <div class="fc-app-report">
            <div class="fc-header">
                <p-toolbar title='Trial Balance' type='3'></p-toolbar>
            </div>
            <div class="fc-content-report">
                <p-filter #filter></p-filter>
            </div>
        </div>
    `,
  providers: [
    FilterItemService,
    FilterItemTraversalService,
    AccountPeriodService,
    TrialBalanceReportService,
  ]
})
export class TrialBalanceReportComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
  @ViewChild('filter', { static: true }) filter: FilterComponent;

  private _subskink = new SubSink();

  constructor(fb: FormBuilder,
    private _router: Router,
    private _trialBalanceReportService: TrialBalanceReportService,
    private _filterItemService: FilterItemService,
    private _traversalService: FilterItemTraversalService,
    private _accountingPeriod: AccountPeriodService) {

    super(fb);
  }

  ngOnDestroy(): void {
    this._subskink.unsubscribe();
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
          return this._trialBalanceReportService.pdf({
            filter: {
              filters: f,
              logic: 'and'
            }
          })
        }
      })

    this.form = this.fb.group({ RecordStatus: 2 });
    this.filter.type = 2;

    this.accountingYears().then((period) => {
      this.currentAccountingYear().then((current) => {

        this._filterItemService.translateForm({
          columns: [
            {
              dataType: 'date', field: 'AccountingDate', filter: true, title: 'Account Post Date',
              filterOptions: {
                control: 'date-period',
              }
            }
          ]
        });
        this.filter.form = this._filterItemService.form;
      })
    });
  }

  private compareFn = (a, b) => {

    if (a.YC < b.YC) return -1;
    if (a.YC > b.YC) return 1;
    return 0;
  }

  private currentAccountingYear() {

    return new Promise<any>((resolve) => {

      let current = null;
      this._subskink.sink = this._accountingPeriod.finds({
        filter: {
          logic: 'and',
          filters: kendoFilter([
            { field: 'IsDefaultYear', value: true, operator: 'eq', logic: 'and' }
          ])
        }
      })
        .subscribe((data: any[]) => {

          if (data)
            current = data[0]['YC'];

          resolve(current);
        })
    });
  }

  private accountingYears() {
    return new Promise<any>((resolve) => {

      let period = [];
      this._subskink.sink = this._accountingPeriod.finds()
        .pipe(
          map(data => data.sort(this.compareFn)),
          map((data: { YC: string, Date1: number }[]) => data),
        )
        .subscribe(data => {

          this._subskink.sink = from(data)
            .pipe(
              map(({ YC }) => YC),
              groupBy(group => group),
            ).subscribe(g => {

              period.push({
                value: g.key,
                text: `${g.key} [${data.filter(x => x.YC == g.key)[0].Date1.toString().substring(0, 4)}]`
              })
              resolve(period);
            })
        })
    })
  }
}

@NgModule({
  declarations: [TrialBalanceReportComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    FilterPanelModule,
    InputsModule,
    PInputIconDirectiveModule,
    RouterModule.forChild([{ path: '', component: TrialBalanceReportComponent }])
  ],
  exports: [TrialBalanceReportComponent]
})
export class Module { }