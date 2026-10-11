import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CommissionReceivedAllocationService } from 'src/@sales/services/commission-received-allocation.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commission-received-allocation-list',
  templateUrl: './commission-received-allocation-list.component.html',
  providers: [
    CommissionReceivedAllocationService
  ]
})
export class CommissionReceivedAllocationListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  public _subsink = new SubSink();

  constructor(
    public _router: Router,
    public _allocationService: CommissionReceivedAllocationService) {
  }

  ngOnInit() {

    this._subsink.sink = this._allocationService.subscribe();
    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') });

    this._gridView.config = {
      key: 'InvoiceSK',
      methodName: 'getPager',
      fetch: this._allocationService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'InvoiceSK', filter: false, title: '', viewDetail: (a: string) => this.detail({ SysKey: a }) },
        { dataType: 'string', field: 'InvoiceNo', filter: true, title: 'Invoice No.', align: 'center', width: 120 },
        { dataType: 'date', field: 'InvoiceDate', filter: true, title: 'Invoice Date' },
        { dataType: 'string', field: 'TransactionNo', filter: true, title: 'Transaction No.', align: 'center', width: 120 },
        { dataType: 'string', field: 'ClientName', filter: true, title: 'Client Name' },
        { dataType: 'string', field: 'Address', filter: true, title: 'Property Address' },
        { dataType: 'currency', field: 'TotalReceivedAmt', filter: false, title: 'Received' },
        { dataType: 'currency', field: 'TotalAllocatedAmt', filter: false, title: 'Allocated' },
        { dataType: 'currency', field: 'Balance', filter: false, title: 'Balance' },
        {
          dataType: 'sales-status', field: 'SalesStatus', filter: true, title: 'Status', filterOptions: {
            defaultValue1: [1, 2],
          }
        },
      ]
    };
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  detail(param: { SysKey?: string }): Promise<any> {
    return this._router.navigateByUrl(`Business/CommissionReceivedAllocationDetail/${param.SysKey}`);
  }
}
