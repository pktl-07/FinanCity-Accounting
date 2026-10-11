import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { CashbookSetupDetailComponent } from '../cashbook-setup-detail/cashbook-setup-detail.component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-cashbook-setup-list.component',
  templateUrl: './cashbook-setup-list.component.html',
  providers: [
    CashbookSetupService,
    CurrencyRateService,
    PWindowComponentService
  ]
})
export class CashbookSetupListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _cashBookSetupService: CashbookSetupService,
    public _windowService: PWindowComponentService,
    public _router: Router,
    public _multiCurrency: CurrencyRateService) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'CBSK',
      fetch: this._cashBookSetupService,
      methodName: 'getPager',
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'CBSK', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'CashbookName', filter: true, title: 'Cashbook Name' },
        {
          dataType: 'string', field: 'CashbookType', filter: true, width: 100, title: 'Cashbook Type', filterOptions: {
            control: 'multi-select',
            source: [{ value: 'CASH', text: 'CASH' }, { value: 'BANK', text: 'BANK' }]
          }
        },
        { dataType: 'string', field: 'BankAccountName', width: 100, filter: true, title: 'Bank Account Name' },
        {
          dataType: 'currcode', field: 'CurrencyCode', filter: true, width: 100, title: 'Base Currency', filterOptions: {
            source: this._multiCurrency
          },
        },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ]
    }
    this._multiCurrency.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'maximized';
      let window = this._windowService.open({ title: 'Cashbook Setup', componentType: CashbookSetupDetailComponent, state: state });
      let windowComponent = window.content.instance as CashbookSetupDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
      resolve(null);
    })
  }
}
