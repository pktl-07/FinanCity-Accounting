import { Component, OnInit, ViewChild } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CommissionReceivedService } from 'src/@sales/services/commission-received.service';

@Component({
  selector: 'app-commission-received-list',
  templateUrl: './commission-received-list.component.html',
  providers: [
    CommissionReceivedService,
  ]
})
export class CommissionReceivedListComponent implements OnInit {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  constructor(
    public _commissionReceivedService: CommissionReceivedService,
    public _router: Router) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') });

    this._gridView.config = {
      key: 'InvoiceSK',
      methodName: 'getPager',
      fetch: this._commissionReceivedService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'InvoiceSK', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'InvoiceNo', filter: true, title: 'Invoice No.', align: 'center', width: 120 },
        { dataType: 'date', field: 'InvoiceDate', filter: true, title: 'Invoice Date' },
        { dataType: 'string', field: 'TransactionNo', filter: true, title: 'Transaction No.', align: 'center', width: 120 },
        { dataType: 'string', field: 'Address', filter: true, title: 'Property Address' },
        { dataType: 'currency', field: 'InvoiceAmt', filter: false, title: 'Invoice Amt.' },
        { dataType: 'currency', field: 'Credit', filter: false, title: 'Credit Amt.' },
        { dataType: 'currency', field: 'Debit', filter: false, title: 'Debit Amt.' },
        { dataType: 'currency', field: 'TotalReceived', filter: false, title: 'Received', },
        { dataType: 'currency', field: 'Balance', filter: false, title: 'Balance' },
        {
          dataType: 'sales-status', field: 'SalesStatus', filter: true, title: 'Status', filterOptions: {
            defaultValue1: [1, 2],
          }
        },
      ]
    };
  }

  createNew(param: { SysKey: string }): Promise<any> {

    return this._router.navigateByUrl(`Business/CommissionReceivedDetail/${param.SysKey}`);
  }
}
