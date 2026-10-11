import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { ClientService } from 'src/@sales/services/client.service';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { ClientManagementDetailComponent } from '../client-management-detail/client-management-detail.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-client-management-list',
  templateUrl: './client-management-list.component.html',
  providers: [ClientService, PWindowComponentService]
})
export class ClientManagementListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  private _unsubscribe: Subject<any> = new Subject();

  constructor(
    public _windowService: PWindowComponentService,
    public _clientService: ClientService,
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
      fetch: this._clientService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'R1', filter: true, title: 'Client No.', width: 120 },
        { dataType: 'checkbox', field: 'IsOrganization', filter: true, title: 'Is Organization' },
        { dataType: 'string', field: 'Name', filter: true, title: 'Client Name' },
        { dataType: 'string', field: 'NRIC', filter: true, title: 'NRIC/ACRA No.', width: 120 },
        { dataType: 'string', field: 'Address', filter: true, title: 'Address', width: 400 },
        { dataType: 'string', field: 'Mobile', filter: true, title: 'Mobile', width: 100 },
        { dataType: 'string', field: 'Email', filter: true, title: 'Email', width: 100 },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ]
    }
  }

  ngOnDestroy(): void {

    this._unsubscribe.complete();
    this._unsubscribe.next();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'maximized';
      let window = this._windowService.open({ title: 'Client Management', componentType: ClientManagementDetailComponent, state: state });
      let windowComponent = window.content.instance as ClientManagementDetailComponent;

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
