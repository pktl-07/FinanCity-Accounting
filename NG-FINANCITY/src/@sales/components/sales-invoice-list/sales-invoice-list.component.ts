import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { SalesInvoiceService } from 'src/@sales/services/sales-invoice.service';
import { recordStatusOptions, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-sales-invoice-list',
  templateUrl: './sales-invoice-list.component.html',
  providers: [SalesInvoiceService,
    CurrencyRateService,
    PWindowComponentService]
})
export class SalesInvoiceListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  public _subsink = new SubSink();

  constructor(
    public _multiCurrency: CurrencyRateService,
    public _router: Router,
    public _windowService: PWindowComponentService,
    public _salesInvoice: SalesInvoiceService) { }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') });

    this._gridView.config = {
      key: 'InvoiceSK',
      fetch: this._salesInvoice,
      filterable: 'window',
      methodName: 'getPager',
      columns: [
        { dataType: 'button', field: 'InvoiceSK', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'InvoiceNo', filter: true, width: 120, align: 'center', title: 'Invoice No.' },
        { dataType: 'date', field: 'InvoiceDate', filter: true, width: 120, title: 'Invoice Date' },
        { dataType: 'string', field: 'RefNo', filter: true, width: 120, align: 'center', title: 'Trans. No.' },
        { dataType: 'string', field: 'Address', filter: true, title: 'Property Address' },
        { dataType: 'string', field: 'ClientName', filter: true, title: 'Client Name' },
        { dataType: 'string', field: 'SalespersonName', filter: true, title: 'Salesperson Name' },
        { dataType: 'string', field: 'MortgageBank', filter: true, title: 'Bank' },
        { dataType: 'currency', field: 'InvoiceAmt', filter: false, title: 'Invoice Amt.' },
        {
          dataType: 'record_status', field: 'RecordStatus', filter: true, title: 'Status', filterOptions: {
            source: recordStatusOptions().filter(x => x.value != -1 && x.value != 125 && x.value != 126),
            defaultValue1: [1, 2, 8],
          }
        },
      ]
    }

    this._multiCurrency.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {
    return this._router.navigateByUrl(`Business/SalesInvoiceDetail/${param.SysKey}`);
  }
}
