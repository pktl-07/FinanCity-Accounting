import { Component, OnInit, ViewChild, ViewEncapsulation, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { IDReferenceIndexService } from 'src/@sales/services/Id-reference-Index-service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { RecordIdIndexerDetailComponent } from '../record-id-indexer-detail/record-id-indexer-detail.component';
import { Router } from '@angular/router';
import { RowClassArgs } from '@progress/kendo-angular-grid';
import { SubSink } from 'src/@common/service/utility-service';

declare var $: any;

@Component({
  selector: 'app-record-id-indexer',
  templateUrl: './record-id-indexer-list.component.html',
  encapsulation: ViewEncapsulation.None,
  styles: [
    ` .k-grid .high-light { 
      background-color : var(--light);
      pointer-event:none;
     }

     .colspan-3{
        column-span:all;
        -webkit-column-span:all;
        -moz-column-span:all;
        -ms-column-span:all;
        -o-column-span:all;
     }
    `
  ],
  providers: [
    IDReferenceIndexService,
    PWindowComponentService
  ]
})
export class RecordIdIndexerListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  private _subsink = new SubSink();

  constructor(
    public _windowService: PWindowComponentService,
    public _indexerService: IDReferenceIndexService,
    public _router: Router) {
  }

  ngOnInit() {

    this._indexerService.asyncFinds();

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') });
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  rowCallback = (context: RowClassArgs) => {

    if (context.dataItem.IsHeader == 1) {

      context.dataItem.StartValue = null;
      context.dataItem.EndValue = null;

      return { 'colspan-3': true };
    }
    return {};
  }

  createNew(param: { SysKey: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'ID Index Reference', componentType: RecordIdIndexerDetailComponent, state: state });
      let windowComponent = window.content.instance as RecordIdIndexerDetailComponent;

      windowComponent.loadData({ SysKey: param.SysKey });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._indexerService.asyncFinds();
        })

      resolve(null);
    })
  }
}
