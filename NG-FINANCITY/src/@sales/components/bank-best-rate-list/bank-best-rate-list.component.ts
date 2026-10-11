import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { from } from 'rxjs';
import { BankBestRateService } from 'src/@sales/services/bank-best-rate.service';
import { BankBestRateDetailComponent } from '../bank-best-rate-detail/bank-best-rate-detail.component';
import { FileService } from 'src/@common/service/file.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-bank-best-rate-list',
  templateUrl: './bank-best-rate-list.component.html',
  providers: [BankBestRateService,
    FileService,
    PWindowComponentService]
})
export class BankBestRateListComponent implements OnInit, OnDestroy {

  private _subsink = new SubSink();

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  constructor(
    public _router: Router,
    public _fileService: FileService,
    public _windowService: PWindowComponentService,
    public _bankBestRateService: BankBestRateService) { }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => from(this.createNew({})) });

    this._gridView.config = {
      key: 'SysKey',
      fetch: this._bankBestRateService,
      filterable: 'window',
      methodName: 'getPager',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'Description', filter: true, title: 'Description' },
        { dataType: 'date', field: 'EffectiveDate', filter: true, title: 'Effective Date' },
        { dataType: 'number', field: 'Rate', filter: true, title: 'Rate' },
        { dataType: 'checkbox', field: 'AttachDocument', filter: false, width: 150, title: 'Add Document' },
      ],
      rowDataBound: (a) => {

        this._subsink.sink = this._fileService.has({ fileType: 5, uploadId: a.data['SysKey'] })
          .subscribe((f => {
            a.data['AttachDocument'] = f;
          }))
      }
    }
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Bank Rates', componentType: BankBestRateDetailComponent, state: state });
      let windowComponent = window.content.instance as BankBestRateDetailComponent;

      windowComponent.loadData({ SysKey: param.SysKey });
      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        })
      resolve(null);
    })
  }
}
