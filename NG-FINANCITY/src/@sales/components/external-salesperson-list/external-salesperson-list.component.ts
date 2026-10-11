import { Component, OnInit, ViewChild, OnDestroy, } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { ExternalSalespersonService } from 'src/@sales/services/external-salesperson.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { vendorType } from 'src/@common/service/utility-service';
import { ExternalSalespersonDetailComponent } from '../external-salesperson-detail/external-salesperson-detail.component';

@Component({
  selector: 'app-external-salesperson-list',
  templateUrl: './external-salesperson-list.component.html',
  providers: [ExternalSalespersonService,
    PWindowComponentService
  ]
})
export class ExternalSalespersonListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _unsubscribe = new Subject<any>();

  constructor(
    public _windowService: PWindowComponentService,
    public _externalSalespersonService: ExternalSalespersonService,
    public _router: Router) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      filterable: 'window',
      methodName: 'getPager',
      fetch: this._externalSalespersonService,
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'R1', filter: true, title: 'Salesperson Number', width: 120 },
        { dataType: 'string', field: 'Name', filter: true, title: 'Salesperson Name' },
        { dataType: 'string', field: 'NRIC', filter: true, title: 'NRIC No.', width: 120 },
        { dataType: 'string', field: 'Designation', filter: true, title: 'Designation', width: 120 },
        { dataType: 'string', field: 'CompanyName', filter: true, title: 'Company Name', width: 350 },
        {
          dataType: 'string', field: 'VendorType', filter: true, title: 'Vendor Type', width: 250, filterOptions: {
            control: "multi-select",
            source: vendorType(),
            textField: 'text',
            valueField: 'value'
          }
        },
        { dataType: 'string', field: 'Mobile', filter: true, title: 'Mobile', width: 120 },
        { dataType: 'string', field: 'Email', filter: true, title: 'Email', width: 120 },
        // { dataType: 'string', field: 'Address', filter: true, title: 'Address', width: 250 },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active', width: 50 },
      ], rowDataBound: (d) => {

        d.data['VendorType'] = vendorType().filter(x => x.value == d.data['VendorType'])[0].text;
      }
    }
  }

  ngOnDestroy(): void {

    this._unsubscribe.next();
    this._unsubscribe.complete();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'maximized';
      let window = this._windowService.open({ title: 'External Salesperson', componentType: ExternalSalespersonDetailComponent, state: state });
      let windowComponent = window.content.instance as ExternalSalespersonDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      windowComponent.OnSuccess.pipe(takeUntil(this._unsubscribe)).subscribe(() => {
        this._gridView.refresh();
      })

      resolve(null);
    })
  }
}
