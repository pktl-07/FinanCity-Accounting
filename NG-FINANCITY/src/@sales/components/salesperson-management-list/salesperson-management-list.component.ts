import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { ButtonControlBuilder } from 'src/@common/components/p-toolbar/button-control.builder';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { SalespersonManagementDetailComponent } from '../salesperson-management-detail/salesperson-management-detail.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { DepartmentService } from 'src/@accounts/services/department.service';
import { VendorService } from 'src/@sales/services/vendor.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-salesperson-management-list',
  templateUrl: './salesperson-management-list.component.html',
  providers: [
    SalespersonManagementService,
    PWindowComponentService,
    DepartmentService,
    VendorService]
})
export class SalespersonManagementListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild('salespersonManagementGrid', { static: true })

  _gridView: PGridViewComponent;

  private _subsink = new SubSink()

  constructor(
    public _buttonControlBuilder: ButtonControlBuilder,
    public _windowService: PWindowComponentService,
    public _vndorService: VendorService,
    public _departmentService: DepartmentService,
    public _salespersonManagementService: SalespersonManagementService,
    public _router: Router) {
  }

  ngOnInit() {

    this._subsink.sink = this._salespersonManagementService.subscribe();
    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      fetch: this._salespersonManagementService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'R1', filter: true, title: 'Salesperson No.', width: 120 },
        { dataType: 'string', field: 'Name', filter: true, title: 'Salesperson Name', width: 240 },
        { dataType: 'string', field: 'NRIC', filter: true, title: 'NRIC No.', width: 120 },
        { dataType: 'string', field: 'ManagerName', filter: true, title: 'Manager', width: 240 },
        { dataType: 'string', field: 'DirectorName', filter: true, title: 'Director', width: 240 },
        {
          dataType: 'string', field: 'Department', filter: true, title: 'Division', width: 120, filterOptions: {
            control: 'multi-select',
            source: this._departmentService,
            valueField: 'Description',
            textField: 'Description'
          }
        },
        { dataType: 'string', field: 'CommissionScheme', filter: true, title: 'Commission Scheme', width: 240 },
        { dataType: 'string', field: 'MobilePhone', filter: true, title: 'Mobile Phone', width: 100 },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active', width: 100 },
      ],
    }

    this._vndorService.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([{ field: 'TransType', value: '6', logic: 'and', operator: 'eq' }])
      }
    })

    this._departmentService.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }) {

    return new Promise((resolve) => {
      let state: WindowState = 'maximized';
      let window = this._windowService.open({
        title: 'Salesperson Management',
        componentType: SalespersonManagementDetailComponent,
        state: state
      });
      let windowComponent = window.content.instance as SalespersonManagementDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });
      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
      resolve(null);
    })
  }
}
