import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { CashbookTransferService } from 'src/@accounts/services/cashbook-transfer.service';
import { recordStatusOptions, SubSink } from 'src/@common/service/utility-service';
import { CashbookTransferDetailComponent } from '../cashbook-transfer-detail/cashbook-transfer-detail.component';

@Component({
  selector: 'app-cashbook-transfer-list.component',
  templateUrl: './cashbook-transfer-list.component.html',
  providers: [
    CashbookSetupService,
    PWindowComponentService,
    CashbookTransferService
  ]
})
export class CashbookTransferListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _cashBookSetupService: CashbookSetupService,
    public _cashbookTransferService: CashbookTransferService,
    public _windowService: PWindowComponentService,
    public _router: Router) {
  }

  ngOnInit() {
    // this._cashBookSetupService.asyncFinds();
    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      fetch: this._cashbookTransferService,
      methodName: 'getPager',
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'R1', filter: true, align: 'center', title: 'Transaction ID.', width: 100 },
        { dataType: 'string', field: 'JRef', filter: true, align: 'center', title: 'GL Ref.', width: 100 },
        {
          dataType: 'date', field: 'TransactionDate', filter: true, width: 100, title: 'Transaction Date'
        },
        {
          dataType: 'string', field: 'FromCBSysKey', width: 150, filter: true, title: 'From Cashbook', filterOptions: {
            control: 'dropdown',
            source: this._cashBookSetupService,
            valueField: 'SysKey',
            textField: 'Description',
          }
        },
        {
          dataType: 'string', field: 'ToCBSysKey', width: 150, filter: true, title: 'To Cashbook', filterOptions: {
            control: 'dropdown',
            source: this._cashBookSetupService,
            valueField: 'SysKey',
            textField: 'Description',
          }
        },
        {
          dataType: 'string', field: 'Description', filter: true, title: 'Transaction Description'
        },
        { dataType: 'currency', field: 'FromTransactionAmount', width: 150, filter: false, title: 'Total Amount' },
        {
          dataType: 'record_status', field: 'RecordStatus', filter: true, title: 'Status', filterOptions: {
            source: recordStatusOptions().filter(x => x.value != 1 && x.value != 125 && x.value != 126 && x.value != -1),
            defaultValue1: [2, 8],
          },
        },
      ],
      rowDataBound: (bound => {

        let fromCashbookName = (this._cashBookSetupService.value as any[]).find(f => f.SysKey == bound.data['FromCBSysKey']).CashbookName;
        if (this._cashBookSetupService.value)
          bound.data['FromCBSysKey'] = fromCashbookName;

        let tocashbookName = (this._cashBookSetupService.value as any[]).find(f => f.SysKey == bound.data['ToCBSysKey']).CashbookName;
        if (this._cashBookSetupService.value)
          bound.data['ToCBSysKey'] = tocashbookName;

      })
    }

    this._cashBookSetupService.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Cashbook Transfer', componentType: CashbookTransferDetailComponent, state: state });
      let windowComponent = window.content.instance as CashbookTransferDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
      resolve(null);
    })
  }
}
