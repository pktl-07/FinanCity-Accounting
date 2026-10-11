import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { VendorService } from 'src/@sales/services/vendor.service';
import { VendorManagementDetailComponent } from '../vendor-management-detail/vendor-management-detail.component';
import { SubSink, vendorType } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-vendor-management-list',
  templateUrl: './vendor-management-list.component.html',
  providers: [
    VendorService,
    PWindowComponentService
  ]
})
export class VendorManagementListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  public _subsink = new SubSink();

  constructor(
    public _windowService: PWindowComponentService,
    public _vendorService: VendorService,
    public _router: Router) { }

  _vendorType = [];

  ngOnInit() {

    this._vendorType = vendorType();

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      filterable: 'window',
      fetch: this._vendorService,
      columns: [
        {
          dataType: 'hidden', field: 'VendorType', filter: true, title: 'Vendor Type', filterOptions: {
            operator: 'neq',
            defaultValue1: 6,
          }
        },
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'VendorNo', filter: true, title: 'Vendor No.', width: 120 },
        {
          dataType: 'string', field: 'VendorType', filter: true, title: 'Vendor Type', width: 120, filterOptions: {
            control: 'multi-select',
            source: vendorType()
          }
        },
        { dataType: 'string', field: 'CompanyName', filter: true, title: 'Company Name' },
        { dataType: 'string', field: 'ACRANo', filter: true, title: 'ACRA No.', width: 120 },
        { dataType: 'string', field: 'Address', filter: true, title: 'Address', width: 200 },
        { dataType: 'string', field: 'ContactPerson', filter: true, title: 'Contact Person', width: 200 },
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
      let window = this._windowService.open({ title: 'Vendor Management', componentType: VendorManagementDetailComponent, state: state });
      let windowComponent = window.content.instance as VendorManagementDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });

      resolve(null);
    })
  }
}
