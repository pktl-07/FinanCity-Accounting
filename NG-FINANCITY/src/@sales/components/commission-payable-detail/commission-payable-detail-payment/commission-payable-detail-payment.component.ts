import { Component, OnInit, ViewChild, OnDestroy, Input } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { CommissionPayableDetailPaidComponent } from '../commission-payable-detail-paid/commission-payable-detail-paid.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { PayableSchemeService } from 'src/@sales/services/payable-scheme.service';
import { kendoFilter, multiCurrency, SubSink } from 'src/@common/service/utility-service';
import { CurrencyService } from 'src/@sales/services/currency.service';

@Component({
  selector: 'app-commission-payable-detail-payment',
  templateUrl: './commission-payable-detail-payment.component.html',
  providers: [
    PayableSchemeService,
    CommissionPayableService,
    PWindowComponentService,
    CurrencyService
  ]
})
export class CommissionPayableDetailPaymentComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild('commissionPayment', { static: true }) _gridView: PGridViewComponent;
  @Input('InvoiceSK') public InvoiceSK: string;
  @Input('DetailSK') public DetailSK: string;
  @Input('SPSK') public SPSK: string;
  @Input('ALOSK') public ALOSK: string;
  @Input('CommissionPayable') public commissionPayable: number;
  @Input('MarketingPromotion') public marketingPromotion: number;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    private _multiCurrency: CurrencyService,
    public _windowService: PWindowComponentService,
    public _payableSchemeService: PayableSchemeService,
    public _commissionPayableService: CommissionPayableService) {

    super(fb);
  }

  ngOnInit() {

    this._gridView.pageable = false;
    this._gridView.config = {
      key: 'SysKey',
      methodName: 'asyncFinds',
      filterable: 'none',
      fetch: this._commissionPayableService,
      insertable: () => this.createNew({}),
      columns: [
        { dataType: 'hidden', field: 'SysKey', filter: true, title: '' },
        {
          dataType: 'hidden', field: 'ALOSK', filter: true, title: '', filterOptions: {
            defaultValue1: this.ALOSK
          }
        },
        {
          dataType: 'hidden', field: 'TotalPayable', filter: false, title: ''
        },
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string, dataItem: any) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'R1', filter: true, title: 'Payment No.', align: 'center' },
        { dataType: 'string', field: 'JRef', filter: true, title: 'JE No.', align: 'center', width: 120 },
        { dataType: 'date', field: 'DatePaid', filter: true, title: 'Date', align: 'center' },
        { dataType: 'currency', field: 'NetPaid', filter: true, title: 'Amount Paid' },
        // {
        //   dataType: 'string', field: 'CURRSK', align: 'center', filter: false, width: 120, title: 'Currency'
        // },
        { dataType: 'record_status', field: 'RecordStatus', filter: false, title: 'Status' },
      ], rowDataBound: d => {

        this._subsink.sink = this._multiCurrency.findByKey(d.data['CURRSK'])
          .subscribe((curr) => {
            d.data['CURRSK'] = curr['Currency_AlphaCode'];
          });

        d.data['NetPaid'] = multiCurrency({
          value: d.data['NetPaid'],
          op: d.data['CURROP'],
          rate: d.data['CURRATE']
        });

        if (d.data['RecordStatus'] == 8) {
          this._subsink.sink = this._payableSchemeService.finds({
            filter: {
              filters: this._getPaidCriteria(),
              logic: 'and'
            }
          }).subscribe((payable: []) => {
            payable.reduce((sum, current) => sum + current['AssociateComm'], 0);
            d.data['SharePercent'] = (d.data['TotalPayable'] / d.data['NetPaid']) * 100;
          });
        } else {
          // d.data['SharePercent'] = '0';
        }
      }
    }
  }

  private _getPaidCriteria() {

    return kendoFilter([
      { field: 'ALOSK', value: this.ALOSK, operator: 'eq', logic: 'and' },
      { field: 'RecordStatus', value: 8, operator: 'eq', logic: 'and' }
    ])
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>(async (resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Commission Payable', componentType: CommissionPayableDetailPaidComponent, state: state });
      let windowComponent = window.content.instance as CommissionPayableDetailPaidComponent;

      window.window.instance.height = 700;
      windowComponent.ALOSK = this.ALOSK;
      windowComponent.InvoiceSK = this.InvoiceSK;
      windowComponent.SPSK = this.SPSK;
      windowComponent.MarketingExpense = this.marketingPromotion;
      windowComponent.DetailSK = this.DetailSK;

      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
      resolve(null);
    })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe()
  }
}
