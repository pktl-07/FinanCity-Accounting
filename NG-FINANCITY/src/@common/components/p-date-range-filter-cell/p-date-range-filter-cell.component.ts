import { Component, Input, OnInit, OnDestroy, ElementRef } from '@angular/core';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { FilterService, SinglePopupService, PopupCloseEvent } from '@progress/kendo-angular-grid';
import { addDays } from '@progress/kendo-date-math';
import { IntlService } from '@progress/kendo-angular-intl';
import { SubSink } from 'src/@common/service/utility-service';

const closest = (node: any, predicate: any): any => {
  while (node && !predicate(node)) {
    node = node.parentNode;
  }
  return node;
};

@Component({
  selector: 'p-date-range-filter',
  template: `
      <ng-container *ngIf="showOperators == true">
        <div class="k-form">
            <label class="k-form-field">
                <span>From</span>
                <kendo-datepicker (valueChange)="onStartChange($event)"
                  [format]="'dd-MM-yyyy'"
                  [(ngModel)]="start" [max]="max" [popupSettings]="popupSettings">
                </kendo-datepicker>
            </label>
            <label class="k-form-field">
                <span>To</span>
                <kendo-datepicker (valueChange)="onEndChange($event)"
                  [format]="'dd-MM-yyyy'"
                  [(ngModel)]="end" [min]="min" [popupSettings]="popupSettings">
                </kendo-datepicker>
            </label>
        </div>
     </ng-container>
   `,
  styles: [`
        .k-form {
            padding: 5px;
        }
   `],
  providers: [
    SinglePopupService
  ]
})
export class DateRangeFilterCellComponent implements OnInit, OnDestroy {

  @Input() public filter: CompositeFilterDescriptor | any;
  @Input() public filterService: FilterService;
  @Input() public field: string;
  @Input() public showOperators: boolean = true;

  public start: any;
  public end: any;
  private _subsink = new SubSink();

  public get min(): any {
    return this.start ? addDays(this.start, 1) : null;
  }

  public get max(): any {
    return this.end ? addDays(this.end, -1) : null;
  }

  public popupSettings: any = {
    popupClass: 'date-range-filter'
  };

  constructor(public element: ElementRef,
    public _intl: IntlService,
    public popupService: SinglePopupService) {

    // Handle the service onClose event and prevent the menu from closing when the datepickers are still active.
    this._subsink.sink = popupService.onClose
      .subscribe((e: PopupCloseEvent) => {
        if (document.activeElement && closest(document.activeElement,
          node => node === this.element.nativeElement || (String(node.className).indexOf('date-range-filter') >= 0))) {
          e.preventDefault();
        }
      });
  }

  ngOnInit(): void {

    this.start = this.transform(this.findValue('gte'));
    this.end = this.transform(this.findValue('lte'));
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  onStartChange(value: any): void {

    this.filterRange(value, this.end);
  }

  onEndChange(value: any): void {

    this.filterRange(this.start, value);
  }

  public transform(value: any): any {

    if (value === undefined || value === null) {

      return null;
    }
    else if (typeof (value) === 'number') {

      value = value.toString();
    }

    return new Date(Date.parse(value.substring(0, 4) + "-" + value.substring(4, 6) + "-" + value.substring(6, 8)));
  }


  public findValue(operator) {
    const filter = this.filter.filters.filter(x => x.field === this.field && x.operator === operator)[0];
    return filter ? filter.value : null;
  }

  public filterRange(start, end) {
    const filters = [];

    if (start && (!end || start < end)) {
      filters.push({
        field: this.field,
        operator: "gte",
        value: this._intl.formatDate(start, 'yyyyMMdd')
      });
      this.start = start;
    }

    if (end && (!start || start < end)) {
      filters.push({
        field: this.field,
        operator: "lte",
        value: this._intl.formatDate(end, 'yyyyMMdd')
      });
      this.end = end;
    }

    this.filterService.filter({
      logic: "and",
      filters: filters
    });
  }
}
