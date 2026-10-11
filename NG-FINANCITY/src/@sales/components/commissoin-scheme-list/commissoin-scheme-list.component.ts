import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { CommissionSchemeDetailComponent } from '../commission-scheme-detail/commission-scheme-detail.component';
import { Router } from '@angular/router';
import { CommissionStructureService } from 'src/@sales/services/commission-structure.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commissoin-scheme-list',
  templateUrl: './commissoin-scheme-list.component.html',
  providers: [CommissionStructureService,
    PWindowComponentService]
})
export class CommissoinSchemeListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  private _subsink = new SubSink()

  constructor(
    public _windowService: PWindowComponentService,
    public _commissionSchemeService: CommissionStructureService,
    public _router: Router) {
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      fetch: this._commissionSchemeService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'SchemeName', filter: true, title: 'Commission Scheme' },
        { dataType: 'string', field: 'BaseHeaderSK', filter: false, title: 'Commission Type', align: 'center', width: 140 },
        { dataType: 'number', field: 'MaxMB', filter: true, title: 'Over-Riding Levels', align: 'center', width: 120 },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ],
      rowDataBound: (d) => {
        if (d.data['BaseHeaderSK'] == '02') {
          d.data['BaseHeaderSK'] = 'Tier';
        }
        else if (d.data['BaseHeaderSK'] == '01') {
          d.data['BaseHeaderSK'] = 'Fix';
        }
      }
    }
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'maximized';
      let window = this._windowService.open({ title: 'Commission Structure Setup', componentType: CommissionSchemeDetailComponent, state: state });
      let windowComponent = window.content.instance as CommissionSchemeDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });

      resolve(null);
    })
  }
}
