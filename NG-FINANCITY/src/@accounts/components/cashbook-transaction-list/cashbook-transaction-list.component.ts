import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { recordStatusOptions, multiCurrency, SubSink } from 'src/@common/service/utility-service';
import { CashbookTransactionService } from 'src/@accounts/services/cashbook-transaction.service';

@Component({
  selector: 'app-cashbook-transaction-list',
  templateUrl: './cashbook-transaction-list.component.html',
  providers: [
    CashbookTransactionService,
    CurrencyRateService,
  ]
})
export class CashbookTransactionListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _router: Router,
    public _multiCurrency: CurrencyRateService,
    public _cashbookTransactionService: CashbookTransactionService) { }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.detail({}) });

    this._gridView.config = {
      key: 'ParentId',
      methodName: 'getPager',
      fetch: this._cashbookTransactionService,
      filterable: 'window',
      columns: [
        {
          dataType: 'button',
          field: 'ParentId',
          filter: false,
          title: '', viewDetail: (a: string, item: any) => this.detail({ ParentId: a, type: item.TransType })
        },
        {
          dataType: 'string',
          field: 'ReferenceNo',
          filter: true, width: 120,
          title: 'Trans. No.'
        },
        {
          dataType: 'date',
          field: 'TransactionDate',
          filter: true,
          width: 120,
          title: 'Trans. Date'
        },
        {
          dataType: 'string',
          field: 'TransType',
          filter: true,
          width: 120,
          align: 'center',
          title: 'Type',
          filterOptions: {
            control: 'multi-select',
            source: [{ text: 'CBR', value: 205 }, { text: 'CBP', value: 210 }],
            defaultValue1: [205, 210],
          }
        },
        {
          dataType: 'string',
          field: 'JRef',
          filter: true,
          align: 'center',
          width: 120,
          title: 'GL Ref.'
        },
        {
          dataType: 'string',
          field: 'SecondRef',
          width: 120,
          filter: true,
          title: 'Second Ref.'
        },
        {
          dataType: 'string',
          field: 'CashbookName',
          width: 120,
          filter: true,
          title: 'Cashbook'
        },
        {
          dataType: 'string',
          field: 'TransDescription',
          filter: true,
          title: 'Trans. Description'
        },
        {
          dataType: 'currency',
          field: 'TotalNetAmount',
          filter: false,
          title: 'Total Amount'
        },
        {
          dataType: 'currcode',
          field: 'CurrencyCode',
          filter: true,
          width: 120,
          title: 'Currency',
          filterOptions: {
            source: this._multiCurrency
          },
        },
        {
          dataType: 'record_status',
          field: 'RecordStatus',
          filter: true,
          title: 'Status', filterOptions: {
            source: recordStatusOptions().filter(x => x.value != -1 && x.value != 1 && x.value != 125 && x.value != 126),
            defaultValue1: [1, 2, 8],
          },
        },
      ], rowDataBound: (b => {

        b.data['TotalNetAmount'] = multiCurrency({
          op: b.data['CURROP'],
          rate: b.data['CURRATE'],
          value: b.data['TotalNetAmount']
        });

        b.data['TransType'] = b.data['TransType'] == 210 ? 'CBP' : 'CBR';
      })
    }
    this._multiCurrency.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  detail(param: { ParentId?: string, type?: string }) {

    if (param.ParentId) return this._router.navigate([`Cashbook/CashbookTransactionDetail/${param.ParentId}/${param.type == 'CBR' ? 205 : 210}`]);
    else return this._router.navigate([`Cashbook/CashbookTransactionDetail`]);
  }
}
