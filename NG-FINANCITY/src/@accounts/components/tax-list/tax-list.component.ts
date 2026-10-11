import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { TaxDetailComponent } from '../tax-detail/tax-detail.component';
import { TaxService } from 'src/@accounts/services/tax.service';
import { taxType, taxCategory, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-tax-list',
  templateUrl: './tax-list.component.html',
  providers: [
    TaxService,
    PWindowComponentService
  ]
})
export class TaxListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _taxService: TaxService,
    public _windowService: PWindowComponentService,
    public _router: Router) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({
        action: () => this.createNew({})
      });

    this._gridView.config = {
      key: 'SysKey',
      fetch: this._taxService,
      methodName: 'getPager',
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'TaxCode', filter: true, title: 'Tax Code', width: 100 },
        { dataType: 'string', field: 'Description', filter: true, title: 'Tax Name' },
        { dataType: 'number', field: 'TaxPercent', filter: true, align: 'center', title: 'Tax Rate(%)', width: 100 },
        { dataType: 'string', field: 'InputTaxSK', filter: true, title: 'Input Account Code', width: 100 },
        { dataType: 'string', field: 'OutputTaxSK', filter: true, title: 'Output Account Code', width: 100 },
        {
          dataType: 'string', field: 'TaxType', filter: true, title: 'Tax Type', width: 100, filterOptions: {
            control: 'multi-select',
            source: taxType()
          }
        },
        {
          dataType: 'string', field: 'TaxCategory', filter: true, title: 'Tax Category', width: 100, filterOptions: {
            control: 'multi-select',
            source: taxCategory()
          }
        },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ]
    }
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'maximized';
      let window = this._windowService.open({ title: 'GST Setup', componentType: TaxDetailComponent, state: state });
      let windowComponent = window.content.instance as TaxDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
      resolve(null);
    })
  }
}
