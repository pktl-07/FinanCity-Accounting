import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { DepartmentDetailComponent } from '../department-detail/department-detail.component';
import { DepartmentService } from 'src/@accounts/services/department.service';
import { ClientService } from 'src/@sales/services/client.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-department-list',
  templateUrl: './department-list.component.html',
  providers: [
    ClientService,
    PWindowComponentService
  ]
})
export class DepartmentListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _windowService: PWindowComponentService,
    public _departmentService: DepartmentService,
    public _router: Router) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      fetch: this._departmentService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'R1', filter: true, title: 'Division No.', width: 100 },
        { dataType: 'string', field: 'Description', filter: true, title: 'Description' },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active', width: 50 },
      ]
    }
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Division Setup', componentType: DepartmentDetailComponent, state: state });
      let windowComponent = window.content.instance as DepartmentDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });

      resolve(null);
    })
  }
}
