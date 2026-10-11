import { Component, Input, OnInit, OnDestroy, ElementRef } from '@angular/core';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { FilterService, SinglePopupService, PopupCloseEvent } from '@progress/kendo-angular-grid';
import { SubSink } from 'src/@common/service/utility-service';

const closest = (node: any, predicate: any): any => {
    while (node && !predicate(node)) {
        node = node.parentNode;
    }
    return node;
};

@Component({
    selector: 'p-currency-range-filter',
    template: `
        <div class="k-form">
            <label class="k-form-field">
                <span>From</span>
                <kendo-numerictextbox (valueChange)="onStartChange($event)"
                  [format]="'c2'"
                  [value]="start" [max]="max">
                </kendo-numerictextbox>
            </label>
            <label class="k-form-field">
                <span>To</span>
                <kendo-numerictextbox (valueChange)="onEndChange($event)"
                  [format]="'c2'"
                  [value]="end" [min]="min">
                </kendo-numerictextbox>
            </label>
        </div>
   `,
    styles: [`
        .k-form {
            padding: 5px;
        }
   `],
    providers: [SinglePopupService]
})
export class CurrencyRangeFilterCellComponent implements OnInit, OnDestroy {

    @Input() public filter: CompositeFilterDescriptor | any;
    @Input() public filterService: FilterService;
    @Input() public field: string;

    public start: any;
    public end: any;

    public get min(): any {
        return this.start;
    }

    public get max(): any {
        return this.end;
    }

    public popupSettings: any = {
        popupClass: 'date-range-filter'
    };

    private _subsink = new SubSink();
    constructor(public element: ElementRef,

        public popupService: SinglePopupService) {
        this._subsink.sink = popupService.onClose
            .subscribe((e: PopupCloseEvent) => {
                if (document.activeElement && closest(document.activeElement,
                    node => node === this.element.nativeElement || (String(node.className).indexOf('numeric-range-filter') >= 0))) {
                    e.preventDefault();
                }
            });
    }

    public ngOnInit(): void {

        this.start = this.transform(this.findValue('gte'));
        this.end = this.transform(this.findValue('lte'));
    }

    public ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    public onStartChange(value: number): void {
        this.filterRange(value, this.end);
    }

    public onEndChange(value: number): void {
        this.filterRange(this.start, value);
    }

    public transform(value: number): any {

        if (value === undefined || value === null) {

            return null;
        }
        else if (typeof (value) === 'number') {

            value = value;
        }
        return value;
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
                // value: this.generalService.toStringDate(start)
                value: start
            });
            this.start = start;
        }

        if (end && (!start || start < end)) {
            filters.push({
                field: this.field,
                operator: "lte",
                // value: this.generalService.toStringDate(end)
                value: end
            });
            this.end = end;
        }

        this.filterService.filter({
            logic: "and",
            filters: filters
        });
    }
}
