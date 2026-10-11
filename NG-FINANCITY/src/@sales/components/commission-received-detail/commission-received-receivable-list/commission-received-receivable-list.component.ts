import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CommissionReceivedService } from 'src/@sales/services/commission-received.service';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { CommissionReceivedDetailDetailComponent } from '../commission-received-detail-detail/commission-received-detail-detail.component';
import { paymentType, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commission-received-receivable-list',
  templateUrl: './commission-received-receivable-list.component.html',
  providers: [
    CurrencyService,
    CommissionReceivedService,
    PWindowComponentService
  ]
})
export class CommissionReceiveReceivableListComponent implements OnDestroy, OnInit {

  private _subsink = new SubSink();

  @Input() formGroup: FormGroup;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  constructor(
    public _currencyService: CurrencyService,
    public _windowService: PWindowComponentService,
    public _commissionReceivedService: CommissionReceivedService) {
  }

  ngOnInit(): void {

    this._gridView.pageable = false;
    this._gridView.config = {
      key: 'SysKey',
      methodName: 'asyncFinds',
      filterable: 'none',
      fetch: this._commissionReceivedService,
      insertable: () => this.createNew({ SysKey: null }),
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        {
          dataType: 'hidden', field: 'InvoiceSK', filter: true, title: '', width: 120, filterOptions: {
            defaultValue1: this.formGroup.get('InvoiceSK').value
          }
        },
        { dataType: 'string', field: 'R1', filter: true, title: 'Receipt No.', align: 'center', width: 120 },
        { dataType: 'string', field: 'JRef', filter: true, title: 'JE No.', align: 'center', width: 120 },
        { dataType: 'date', field: 'ReceiptDate', filter: true, title: 'Date', align: 'center', width: 120 },
        { dataType: 'string', field: 'PaymentType', filter: true, title: 'Receipt Type' },
        { dataType: 'currency', field: 'TotalAmt', filter: true, width: 150, title: 'Amount Received' },
        { dataType: 'currcode', field: 'CURRSK', filter: true, width: 80, title: 'Currency' },
        { dataType: 'record_status', field: 'RecordStatus', width: 120, filter: false, title: 'Status' },
      ],
      rowDataBound: (d) => {

        d.data['PaymentType'] = paymentType().find(x => x.value == d.data['PaymentType']).text;
        this._subsink.sink = this._currencyService.findByKey(d.data['CURRSK'])
          .subscribe(data => {
            d.data['CURRSK'] = data['Currency_AlphaCode']
          })
      }
    }
  }

  createNew(param: { SysKey: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Commission Receivable', componentType: CommissionReceivedDetailDetailComponent, state: state });
      let windowComponent = window.content.instance as CommissionReceivedDetailDetailComponent;

      windowComponent.invoiceSK = this.formGroup.get('InvoiceSK').value;
      window.window.instance.height = 700;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });

      resolve(null);
    })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
