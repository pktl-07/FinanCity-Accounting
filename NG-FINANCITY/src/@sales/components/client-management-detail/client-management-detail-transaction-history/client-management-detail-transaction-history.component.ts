import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { PropertyTransactionService } from 'src/@sales/services/property-transaction.service';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';

@Component({
  selector: 'app-client-management-detail-transaction-history',
  template :`<p-grid-view></p-grid-view>`,
  providers: [
    PropertyTransactionService
  ]
})
export class ClientManagementTransactionHistoryComponent implements OnInit {

  @Input() ClientSK: string;

  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  constructor(private _propertyTransactionService: PropertyTransactionService) {
  }

  ngOnInit() {

    this._gridView.config = {
      key: 'PropertySK',
      methodName: 'getPager',
      fetch: this._propertyTransactionService,
      filterable: 'none',
      columns: [
        {
          dataType: 'hidden', field: 'ClientSK', filter: true, title: '', filterOptions: {
            defaultValue1: this.ClientSK
          }
        },
        {
          dataType: 'hidden', field: 'InvoiceStatus', filter: true, title: '', filterOptions: {
            defaultValue1: 8
          }
        },
        //InvoiceNo
        { dataType: 'string', field: 'InvoiceNo', filter: true, width: 120, title: 'Invoice Number' },
        { dataType: 'string', field: 'SalesNo', filter: true, width: 120, title: 'Transaction Number' },
        { dataType: 'date', field: 'TransactionDate', filter: true, title: 'Transaction Date' },
        { dataType: 'string', field: 'PropertyAddress', filter: true, title: 'Property Address' },
        { dataType: 'currency', field: 'LoanAmount', filter: true, width: 120, title: 'Loan Amount' },
        { dataType: 'string', field: 'MortgageBank', filter: true, width: 250, title: 'Bank' },
        //marketing promotion
        { dataType: 'currency', field: 'MarketingPromotion', filter: true, width: 120, title: 'Marketing Promotion' },
        { dataType: 'string', field: 'SalespersonName', filter: true, width: 250, title: 'Salesperson' },
      ]
    }
  }
}
