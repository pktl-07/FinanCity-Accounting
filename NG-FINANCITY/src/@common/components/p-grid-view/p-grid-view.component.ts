import { Component, OnInit, Input, ViewEncapsulation, ViewChild, OnDestroy, AfterViewInit, ViewContainerRef, TemplateRef, ApplicationRef, EmbeddedViewRef, ContentChild, Optional } from '@angular/core';
import { State, CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { DataStateChangeEvent, RowClassArgs, GridComponent } from '@progress/kendo-angular-grid';
import { PGridViewConfig } from 'src/@common/Infrastructure/common-interface';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { GridViewService } from './p-grid-view.service';
import { FilterComponent } from '../p-filter-panel/p-filter-panel-component';
import { DialogComponent } from '@progress/kendo-angular-dialog';
import { FilterItemService } from '../p-filter-panel/p-filter-item-service';
import { FormGroup } from '@angular/forms';
import { PWindowComponentService } from '../p-window/p-window.component.service';
import { catchError } from 'rxjs/operators';
import { FilterItemTraversalService } from '../p-filter-panel/p-filter-item-traverse-service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'p-grid-view',
  templateUrl: './p-grid-view.component.html',
  encapsulation: ViewEncapsulation.None,
  styles: [`

    a:not([href]){
       cursor: pointer; 
       -webkit-user-select: none; 
       -moz-user-select: none; 
       user-select: none;
      }
      
      tr.text-center {
        text-align:center
      }

      tr.text-right {
          text-align:right
      }

      .k-grid .k-grid-content td {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      .orange .color-column {
        background-color : var(--orange);
        text-align:center;
        font-weight: bold;
        color: var(--light);
        border-bottom-width:1px;
      }

      .red .color-column {
        background-color : var(--danger);
        text-align:center;
        font-weight: bold;
        color: var(--light);
        border-bottom-width:1px;
      }

      .green .color-column {
        background-color : var(--success);
        text-align:center;
        font-weight: bold;
        color: var(--light);
        border-bottom-width:1px;
      }
      
      .pupple .color-column {
        background-color : var(--pupple);
        text-align:center;
        font-weight: bold;
        color: var(--light);
        border-bottom-width:1px;
      }

      .gray .color-column {
        background-color : var(--gray);
        text-align:center;
        font-weight: bold;
        color: var(--light);
        border-bottom-width:1px;
      }

   `],
  providers: [
    GridViewService,
    FilterItemService,
    FilterItemTraversalService,
    PWindowComponentService
  ]
})
export class PGridViewComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild(TooltipDirective, { static: true }) tooltipDir: TooltipDirective;
  @ViewChild(GridComponent, { static: true }) gridComponent: GridComponent;
  @ViewChild(DialogComponent, { read: DialogComponent, static: true }) dialogComponent: DialogComponent;
  @ContentChild(TemplateRef, { static: true }) templateRef: TemplateRef<any>;

  @Input() config: PGridViewConfig = { key: '', fetch: null, columns: [], methodName: '' };
  @Input() title: string;
  @Input() pageable: boolean = true;

  private _subsink = new SubSink()

  state: State = {
    skip: 0,
    take: 14,
  };

  public view: any;
  public buttonCount = 5;
  public info = true;
  public type: 'numeric' | 'input' = 'numeric';
  public pageSizes = true;
  public previousNext = true;
  public filterFormGroup: FormGroup;

  constructor(
    public gridViewService: GridViewService,
    public filterItemService: FilterItemService,
    public traversalService: FilterItemTraversalService,
    public dialogService: PWindowComponentService) { }

  @Input() lock: boolean = false;

  ngOnInit(): void {

    this.view = this.config.fetch;

    if (this.pageable) {
      this.gridComponent.pageable = {
        buttonCount: this.buttonCount,
        info: this.info,
        type: this.type,
        previousNext: this.previousNext
      };
    }

    this.filterItemService.translateForm(this.config);
    this.filterFormGroup = this.filterItemService.form;

    this._subsink.sink = this.config.fetch.pipe(
      catchError(error => {
        return error;
      })
    ).subscribe((data) => {
      if (data && data.data != null) {
        if (this.config.rowDataBound) {
          for (let idx = 0; idx < data.data.length; idx++) { // notify handler
            this.config.rowDataBound({ data: data.data[idx], index: idx });
          }
        }
      }
      else if (data != null) {
        if (this.config.rowDataBound) {
          for (let idx = 0; idx < data.length; idx++) { // notify handler
            this.config.rowDataBound({ data: data[idx], index: idx });
          }
        }
      }
    });

    this.refresh();
  }

  ngAfterViewInit() {

    //By default the kendo detail grid first row always open.
    //It is a trick to prevent opening first child detail row.
    this.gridComponent.isDetailExpanded = (data) => {
      this.gridComponent.isDetailExpanded = null;
      return false;
    }
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  refresh() {

    let d = this.traversalService.traverseFilter(this.filterItemService.form.get('formItems').value);

    this.state.skip = 0;
    this.state.filter = {
      filters: d,
      logic: 'and',
    };

    if (this.config.methodName) {
      this.config.fetch[this.config.methodName](this.state);
    }
  }

  dataStateChange(state: DataStateChangeEvent): void {

    this.state = state;

    if (this.config.methodName) {
      this.config.fetch[this.config.methodName](this.state);
    }
  }

  filterChange(filter: CompositeFilterDescriptor): void {

    this.state.skip = 0;
    this.state.filter = filter;

    if (this.config.methodName) {
      this.config.fetch[this.config.methodName](this.state);
    }
  }

  handleItemFilter() {

    let o = this.dialogService.open({
      title: 'Filter',
      componentType: FilterComponent
    });

    const s = o.content.instance as FilterComponent;
    s.form = this.filterFormGroup;

    if (this.config.rowFilter) {
      s.onFilter = this.config.rowFilter;
    }

    this._subsink.sink = s.fetchSubject
      .subscribe(() => {

        this.refresh();
        this.dialogService.close();
      });
  }

  showTooltip(e: MouseEvent): void {

    const element = e.target as HTMLElement;
    if ((element.nodeName === 'TD' || element.nodeName === 'TH')
      && element.offsetWidth < element.scrollWidth) {
      this.tooltipDir.toggle(element);
    } else {
      this.tooltipDir.hide();
    }
  }

  recordStatus = (context: RowClassArgs) => {

    if (context.dataItem.RecordStatus) {
      switch (context.dataItem['RecordStatus']) {
        case -1:
          return { pupple: true };
        case 0:
        case 1:
          return { gray: true };
        case 2:
        case 3:
          return { orange: true };
        case 6:
        case 125:
        case 126:
          return { red: true };
        case 8:
          return { green: true };
      }
    }

    if (context.dataItem.SalesStatus) {
      switch (context.dataItem['SalesStatus']) {
        case 1: return { orange: true };
        case 2: return { green: true };
      }
    }
  };

  statusText(status: number) {

    switch (status) {
      case -1:
        return 'SUBMIT';
      case 0:
      case 1:
        return 'NEW';
      case 2:
      case 3:
        return 'ENTER';
      case 6:
        return 'VOID';
      case 8:
        return 'POST';
      case 125:
        return 'REVERSAL'
      case 126:
        return 'REVERSED'
    }
  }

  salesStatusText(status: number) {

    switch (status) {
      case 1:
        return 'OPEN';
      case 2:
        return 'CLOSE';
      default:
        return 'ERROR';
    }
  }

  onInsertClick(config: PGridViewConfig) {

    config.insertable();
  }

  defaultWidth(d: number, w: number) {

    if (w != undefined)
      return w;

    return d;
  }
}
