import { Component, OnInit, ViewChild } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { PropertyTransactionService } from 'src/@sales/services/property-transaction.service';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { recordStatusOptions, multiCurrency } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-property-transaction-list',
  templateUrl: './property-transaction-list.component.html',
  providers: [
    PropertyTransactionService,
    CurrencyRateService,
  ]
})
export class PropertyTransactionListComponent implements OnInit {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  constructor(
    public _router: Router,
    public _multiCurrency: CurrencyRateService,
    public _propertyTransaction: PropertyTransactionService) { }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.detail({}) });

    this._gridView.config = {
      key: 'PropertySK',
      methodName: 'getPager',
      fetch: this._propertyTransaction,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'PropertySK', filter: false, title: '', viewDetail: (a: string) => this.detail({ SysKey: a }) },
        { dataType: 'string', field: 'SalesNo', filter: true, width: 120, title: 'Trans. No.' },
        { dataType: 'date', field: 'TransactionDate', filter: true, width: 120, title: 'Trans. Date' },
        { dataType: 'string', field: 'PropertyAddress', filter: true, title: 'Property Address' },
        { dataType: 'string', field: 'ClientName', filter: true, title: 'Client Name' },
        { dataType: 'string', field: 'SalespersonName', filter: true, title: 'Salesperson Name' },
        { dataType: 'string', field: 'MortgageBank', filter: true, width: 120, title: 'Bank' },
        { dataType: 'currency', field: 'CommissionAmount', filter: false, width: 120, title: 'Transaction Amount' },
        { dataType: 'currency', field: 'MarketingPromotion', filter: true, width: 120, title: 'Marketing Promotion' },
        {
          dataType: 'currcode', field: 'CurrencyCode', filter: true, width: 120, title: 'Currency', filterOptions: {
            source: this._multiCurrency
          },
        },
        {
          dataType: 'record_status', field: 'RecordStatus', filter: true, title: 'Status', filterOptions: {
            source: recordStatusOptions().filter(x => x.value != 125 && x.value != 126),
            defaultValue1: [-1, 1, 2, 8],
          },
        },
      ], rowDataBound: (b => {

        b.data['CommissionAmount'] = multiCurrency({
          op: b.data['CURROP'],
          rate: b.data['CURRATE'],
          value: b.data['CommissionAmount']
        })
      })
    }

    this._multiCurrency.asyncFinds();
  }

  detail(param: { SysKey?: string }) {

    if (param.SysKey)
      return this._router.navigateByUrl(`Business/PropertyTransactionDetail/${param.SysKey}`);
    else
      return this._router.navigateByUrl(`Business/PropertyTransactionDetail`);
  }
}
