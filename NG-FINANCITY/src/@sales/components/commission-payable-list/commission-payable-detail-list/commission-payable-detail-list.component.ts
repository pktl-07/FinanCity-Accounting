import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { DetailView } from 'src/@common/Infrastructure/common-interface';

@Component({
  selector: 'app-commission-payable-detail-list',
  template: '<p-grid-view #commissionPayableDetail></p-grid-view>',
  providers: [CommissionPayableService]
})
export class CommissionPayableDetailListComponent implements OnInit, DetailView {

  @ViewChild('commissionPayableDetail', { static: true }) public _gridView: PGridViewComponent;
  @Input('dataItem') public dataItem: any;

  constructor(
    public _router: Router,
    public _commissionPayableService: CommissionPayableService) {
  }

  ngOnInit() {

    this._gridView.config = {
      key: 'ALOSK',
      methodName: 'getDetailPager',
      fetch: this._commissionPayableService,
      filterable: 'none',
      columns: [
        { dataType: 'hidden', field: 'InvoiceSK', filter: true, title: '', filterOptions: { defaultValue1: this.dataItem.InvoiceSK } },
        { dataType: 'button', field: 'ALOSK', filter: false, title: '', viewDetail: (key: string, dataItem: any) => this.createNew(key, dataItem['InvoiceSK'], dataItem['SPSK']) },
        { dataType: 'string', field: 'CoBrokerType', filter: false, title: 'Salesperson Type', width: 100 },
        { dataType: 'string', field: 'SPName', filter: true, title: 'Salesperson Name' },
        { dataType: 'string', field: 'NRIC', filter: true, title: 'NRIC No.', align: 'center', width: 120 },
        { dataType: 'string', field: 'Agency', filter: true, title: 'Company Name', width: 200 },
        { dataType: 'currency', field: 'GrossAllocatedAmount', filter: false, title: 'Gross Comm.', width: 120 },
        { dataType: 'currency', field: 'MarketingExpense', filter: false, title: 'Marketing Promotion', width: 120 },
        { dataType: 'currency', field: 'AgencyAmount', filter: false, title: 'Company Fees', width: 120 },
        { dataType: 'currency', field: 'PayableAmount', filter: false, title: 'SP Payable', width: 120 },
        { dataType: 'currency', field: 'PaidAmount', filter: true, title: 'SP Paid', width: 120 },
        { dataType: 'currency', field: 'BalanceAmount', filter: true, title: 'SP Balance', width: 120 },
      ], rowDataBound: (e) => {
        e.data['BalanceAmount'] = e.data['PayableAmount'] - e.data['PaidAmount'];
      }
    };
  }

  createNew(alosk = null, invoicesk = null, spsk = null): Promise<any> {

    return new Promise<any>((resolve) => {
      this._router.navigateByUrl(`Business/CommissionPayableDetail/${alosk}/${invoicesk}/${spsk}`);
      resolve(null);
    })
  }
}
