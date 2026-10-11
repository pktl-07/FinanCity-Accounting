import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { ButtonControlBuilder } from 'src/@common/components/p-toolbar/button-control.builder';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { CommissionPayableDetailListComponent } from './commission-payable-detail-list/commission-payable-detail-list.component';

@Component({
  selector: 'app-commission-payable-list',
  templateUrl: './commission-payable-list.component.html',
  providers: [
    CommissionPayableService,
  ]
})
export class CommissionPayableListComponent implements OnInit, AfterViewInit {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  constructor(
    public _buttonControlBuilder: ButtonControlBuilder,
    public _router: Router,
    public _commissionPayableService: CommissionPayableService) {
  }

  ngAfterViewInit(): void {

    this._gridView.gridComponent.expandRow(0);
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') });

    this._gridView.config = {
      key: 'InvoiceSK',
      methodName: 'getPager',
      fetch: this._commissionPayableService,
      filterable: 'window',
      detailTemplate: CommissionPayableDetailListComponent,
      columns: [
        { dataType: 'hidden', field: 'InvoiceSK', filter: false, title: '' },
        { dataType: 'string', field: 'InvoiceNo', filter: true, title: 'Invoice No.', align: 'center', width: 120 },
        { dataType: 'string', field: 'SalesOrderNo', filter: true, title: 'Transaction No.', width: 120 },
        { dataType: 'string', field: 'Address', filter: true, title: 'Property Address' },
        { dataType: 'currency', field: 'GrossReceivableAmt', filter: false, title: 'Gross Comm.' },
        { dataType: 'currency', field: 'MarketingExpense', filter: false, title: 'Marketing Promotion' },
        { dataType: 'hidden', field: 'GSTValue', filter: false, title: 'GST ($)' },
        { dataType: 'currency', field: 'AgencyComm', filter: false, title: 'Company Fees' },
        { dataType: 'currency', field: 'TotalPayable', filter: false, title: 'Total Payable' },
        { dataType: 'currency', field: 'TotalPaid', filter: false, title: 'Amount Paid' },
        { dataType: 'currency', field: 'Balance', filter: false, title: 'Balance' },
        {
          dataType: 'sales-status', field: 'SalesStatus', filter: true, title: 'Status', filterOptions: {
            defaultValue1: [1, 2],
          }
        },
      ]
    };
  }
}
