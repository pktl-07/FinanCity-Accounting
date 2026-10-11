import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { JobService } from 'src/@accounts/services/job.service';
import { JobDetailComponent } from '../job-detail/job-detail.component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-job-list.component',
  templateUrl: './job-list.component.html',
  providers: [
    JobService,
    PWindowComponentService
  ]
})
export class JobListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild(PGridViewComponent, { static: true }) _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _jobService: JobService,
    public _windowService: PWindowComponentService,
    public _router: Router,
    public _multiCurrency: CurrencyRateService) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      fetch: this._jobService,
      methodName: 'getPager',
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'R1', width: 120, filter: true, title: 'Job No.' },
        { dataType: 'date', field: 'JobCreatedDate', width: 100, filter: true, title: 'Create Date' },
        { dataType: 'date', field: 'StartDate', width: 100, filter: true, title: 'Start Date' },
        { dataType: 'date', field: 'FinishDate', width: 100, filter: true, title: 'End Date' },
        { dataType: 'string', field: 'Description', filter: true, title: 'Job Description' },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ]
    }
    this._multiCurrency.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'Job', componentType: JobDetailComponent, state: state });
      let windowComponent = window.content.instance as JobDetailComponent;

      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });

      resolve(null);
    })
  }
}
