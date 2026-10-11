import { Component, OnInit, AfterContentInit, ContentChild, Input, EventEmitter, Output } from '@angular/core';
import { FCFilterCompositeDirective } from '../filter/fc-filter-composite.directive';
import { FilterConnectorService } from '../validator/fc.filter-connector.service';

/**
 * @example
 *  <fc-searchable-toolbar [connector]="_filterConnectorService">
 *  <fc-filter-composite title="Product">
 *    <fc-filter-item field="ProductId" label="Product Id." type="text"></fc-filter-item>
 *    <fc-filter-item field="ProductName" label="Product Name" type="text"></fc-filter-item>
 *    </fc-filter-item>
 *  </fc-filter-composite>
 * </fc-searchable-toolbar>
 */
@Component({
    selector: 'fc-searchable-toolbar',
    template: `
        
        <ng-container *ngIf="filterCompositeComponent && visible">
            <ng-container *ngTemplateOutlet="filterCompositeComponent"></ng-container>
        </ng-container>

        <ng-template #filterCompositeComponent>
            <ion-toolbar>
                <ion-searchbar debounce="1000" placeholder="Type 3 words to search." (ionChange)='handleSearchableChange($event)' [value]="defaultValue"></ion-searchbar>
            </ion-toolbar> 
        </ng-template>
  `,
})
export class FCSearchableToolbarComponent implements AfterContentInit {

    @Input() connector: FilterConnectorService;
    @Input() cancelable: boolean = false;
    @Input() addable: boolean = false;

    @Output() cancel = new EventEmitter();
    @Output() add = new EventEmitter();
    @Input() visible: boolean = true;
    @Input() defaultValue: string;

    @ContentChild(FCFilterCompositeDirective, { static: true }) filterCompositeComponent: FCFilterCompositeDirective;

    constructor() { }

    ngAfterContentInit(): void {
        this.refresh();
    }

    handleSearchableChange(event: any) {

        if (event.target.value.length > 2 || event.target.value.length == 0) {
            this.defaultValue = this.connector.filterValue = event.target.value;
            let txtSource = this.filterCompositeComponent.filterItemsComponent.filter(x => x.default == false && x.type == 'text');
            txtSource.map((item, index) => {

                if (txtSource.length > 1) {
                    if (index == 0) {
                        item.value_1 = `<# ( #> ${this.defaultValue}`;
                        item.logic = 'or';
                    }
                    else if (txtSource.length - 1 == index) {

                        item.value_1 = `${this.defaultValue} <# ) #>`;
                        item.logic = 'and';
                    }
                    else {

                        item.value_1 = this.defaultValue;
                        item.logic = 'or';
                    }
                } else {

                    item.value_1 = this.defaultValue
                    item.logic = 'or';
                }
            });

            this.filterCompositeComponent.filterItemsComponent.filter(x => x.default == false && x.type != 'text').map(item => {

                item.value_1 = this.defaultValue;
                item.logic = 'and';
            });

            this.refresh();
        }
    }

    refresh() {

        if (this.connector) {
            this.connector.next(this.connector.toKendoFilter(this.filterCompositeComponent.filterItemsComponent));
        }
    }

    onClickAdd() {
        this.add.emit();
    }

    onClickCancel() {
        this.cancel.emit();
    }
}
