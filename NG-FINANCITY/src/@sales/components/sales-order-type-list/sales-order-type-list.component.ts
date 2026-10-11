import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { from } from 'rxjs';
import { SalesOrderTypeDetailComponent } from '../sales-order-type-detail/sales-order-type-detail.component';
import { ReferralSalesOrderTypeService } from 'src/@sales/services/referral-sales-order-type.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-sales-order-type-list',
  templateUrl: './sales-order-type-list.component.html',
  providers: [ReferralSalesOrderTypeService,
    PWindowComponentService]
})
export class SalesOrderTypeComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _router: Router,
    public _windowService: PWindowComponentService,
    public _salesOrderTypeService: ReferralSalesOrderTypeService) { }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => from(this.createNew({})) });

    this._gridView.config = {
      key: 'SysKey',
      fetch: this._salesOrderTypeService,
      filterable: 'window',
      methodName: 'getPager',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'Description', filter: true, title: 'Description' },
        { dataType: 'string', field: 'AccountCode', filter: true, title: 'Income Account Code' },
        { dataType: 'string', field: 'AccountDescription', filter: true, title: 'Account Name' },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ]
    }
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({
        title: 'Sales Oder Type Setup', componentType: SalesOrderTypeDetailComponent,
        state: state,
        width: 700
      });

      let windowComponent = window.content.instance as SalesOrderTypeDetailComponent;

      windowComponent.loadData({ SysKey: param.SysKey });
      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        })
      resolve(null);
    })
  }
}
