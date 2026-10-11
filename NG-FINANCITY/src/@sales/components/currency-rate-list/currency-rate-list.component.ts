import { Component, OnInit, ViewChild, OnDestroy, Inject } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { CurrencyRateDetailComponent } from '../currency-rate-detail/currency-rate-detail.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-currency-rate-list',
  templateUrl: './currency-rate-list.component.html',

  providers: [CurrencyRateService,
    { provide: 'CurrencyRateListModule#CurrencyRateService1', useClass: CurrencyRateService },
    { provide: 'CurrencyRateListModule#CurrencyRateService2', useClass: CurrencyRateService }
  ]
})
export class CurrencyRateListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _windowService: PWindowComponentService,
    @Inject('CurrencyRateListModule#CurrencyRateService1') public _currencyRateService1: CurrencyRateService,
    @Inject('CurrencyRateListModule#CurrencyRateService2') public _currencyRateService2: CurrencyRateService,
    public _router: Router) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      fetch: this._currencyRateService1,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'CurrencyCode', filter: true, align: 'center', title: 'Currency Code', width: 120 },
        { dataType: 'string', field: 'Country', filter: true, title: 'Country' },
        { dataType: 'string', field: 'Description', filter: true, title: 'Currency Name' },
        {
          dataType: 'string', field: 'Operator', filter: true, align: 'center', title: 'Operator', width: 120, filterOptions: {
            control: 'multi-select',
            source: [{ value: '/', text: '/' }, { text: '*', value: '*' }]
          }
        },
        {
          dataType: 'string', field: 'Symbol', filter: true, align: 'center', title: 'Symbol', width: 120, filterOptions: {
            control: 'multi-select',
            source: this._currencyRateService2,
            textField: 'Symbol',
            valueField: 'Symbol'
          }
        },
        { dataType: 'number', field: 'Rate', filter: true, title: 'Exchange Rate', width: 120 },
      ]
    }

    this._currencyRateService2.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>(() => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Currency Setup', componentType: CurrencyRateDetailComponent, state: state });
      let windowComponent = window.content.instance as CurrencyRateDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
    })
  }
}
