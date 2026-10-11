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
    selector: 'p-input-string-filter',
    template: `
    <ng-container *ngIf="showOperators == true">
        <div class="k-form">
            <label class="k-form-field">
                <input kendoTextBox [(ngModel)]="value" (input)="onChange($event)" />
            </label>
        </div>
    </ng-container>
   `,
    styles: [`
        .k-form {
            padding: 5px;
        }
   `]
})
export class StringFilterCellComponent implements OnInit, OnDestroy {

    @Input() public filter: CompositeFilterDescriptor | any;
    @Input() public filterService: FilterService;
    @Input() public field: string;
    @Input() public showOperators: boolean = true;

    public _subsink = new SubSink();

    value: any;

    popupSettings: any = {
        popupClass: 'date-range-filter'
    };

    public popupSubscription: any;

    constructor(public element: ElementRef,
        public popupService: SinglePopupService) {

        // Handle the service onClose event and prevent the menu from closing when the datepickers are still active.
        this._subsink.sink = this.popupSubscription = popupService.onClose
            .subscribe((e: PopupCloseEvent) => {
                if (document.activeElement && closest(document.activeElement,
                    node => node === this.element.nativeElement || (String(node.className).indexOf('date-range-filter') >= 0))) {
                    e.preventDefault();
                }
            });
    }

    ngOnInit(): void {

        this.value = this.findValue('contains');
    }

    onChange(value: any): void {

        this.writeFilter(value.target.value);
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    public findValue(operator) {

        const filter = this.filter.filters.filter(x => x.field === this.field && x.operator === operator)[0];
        return filter ? filter.value : null;
    }

    public writeFilter(value) {
        const filters = [];

        if (value) {
            filters.push({
                field: this.field,
                operator: "contains",
                value: this.value
            });
            this.value = value;
        }

        this.filterService.filter({
            logic: "and",
            filters: filters
        });
    }
}
