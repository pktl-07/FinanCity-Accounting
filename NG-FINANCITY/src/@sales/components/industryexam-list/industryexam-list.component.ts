import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { IndustryExamDetailComponent } from '../industryexam-detail/industryexam-detail.component';
import { IndustryExamService } from 'src/@sales/services/industryexam.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-qualification-list',
  templateUrl: './industryexam-list.component.html',
  providers: [
    IndustryExamService,
    PWindowComponentService]
})
export class IndustryExamListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    public _router: Router,
    public _windowService: PWindowComponentService,
    public _industryExamService: IndustryExamService) { }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      fetch: this._industryExamService,
      filterable: 'window',
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'Description', filter: true, title: 'Professional Experience' },
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
      let window = this._windowService.open({ title: 'Professional Experience Setup', componentType: IndustryExamDetailComponent, state: state });
      let windowComponent = window.content.instance as IndustryExamDetailComponent;
      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        })
      resolve(null);
    })
  }
}
