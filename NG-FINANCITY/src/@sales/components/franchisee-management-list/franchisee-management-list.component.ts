import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { VendorService } from 'src/@sales/services/vendor.service';
import { Subject, from } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FranchiseeManagementDetailComponent } from '../franchisee-management-detail/franchisee-management-detail.component';
import { vendorType } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-franchisee-management-list',
  templateUrl: './franchisee-management-list.component.html',
  providers: [
    VendorService,
    PWindowComponentService
  ]
})
export class FranchiseeManagementListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _unsubscribe = new Subject<any>();

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
          dataType: 'hidden', field: 'VendorType', filter: true, title: 'Vendor Type', width: 0, filterOptions: {
            defaultValue1: 6,
          }
        },
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'VendorNo', filter: true, title: 'Franchisee No.', width: 100 },
        { dataType: 'string', field: 'CompanyName', filter: true, title: 'Company Name' },
        { dataType: 'string', field: 'ACRANo', filter: true, title: 'ACRA No.', width: 200 },
        { dataType: 'string', field: 'Address', filter: true, title: 'Address', width: 200 },
        { dataType: 'string', field: 'ContactPerson', filter: true, title: 'Contact Person', width: 100 },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ]
    }
  }

  ngOnDestroy(): void {

    this._unsubscribe.next();
    this._unsubscribe.complete();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'maximized';
      let window = this._windowService.open({ title: 'Franchise Management', componentType: FranchiseeManagementDetailComponent, state: state });
      let windowComponent = window.content.instance as FranchiseeManagementDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      windowComponent.OnSuccess
        .pipe(takeUntil(this._unsubscribe))
        .subscribe(() => {
          this._gridView.refresh();
        });

      resolve(null);
    })
  }
}
