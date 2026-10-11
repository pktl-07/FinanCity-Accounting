import { Component, ContentChild, Input, Output, EventEmitter } from '@angular/core';
import { FCFilterCompositeDirective } from '../filter/fc-filter-composite.directive';
import { ModalController } from '@ionic/angular';
import { FCFilterModalComponent } from '../filter/fc-filter-modal.component';
import { FilterConnectorService } from '../validator/fc.filter-connector.service';

/**
 * @example
 *  <fc-filter-toolbar [connector]="_filterConnectorService">
 *  <fc-filter-composite title="Product">
 *    <fc-filter-item field="ProductId" label="Product Id." type="text"></fc-filter-item>
 *    <fc-filter-item field="ProductName" label="Product Name" type="text"></fc-filter-item>
 *    <fc-filter-item field="CountingUnit" label="Counting Unit" type="text"></fc-filter-item>
 *    <fc-filter-item field="SalesDate" label="Sales Date" type="date"></fc-filter-item>
 *    <fc-filter-item field="IsActive" label="Is Active" [items]="[{value:true,text:'Yes',{value:false,text:'No'}}]" value="value" text="text"
 *      [value_1]="[true]" type="dropdown">
 *    </fc-filter-item>
 *  </fc-filter-composite>
 * </fc-filter-toolbar>
 */
@Component({
    selector: 'fc-filter-toolbar',
    template: `
        
        <ng-container *ngIf="filterCompositeComponent && visible">
            <ng-container *ngTemplateOutlet="filterCompositeComponent"></ng-container>
        </ng-container>

        <ng-template #filterCompositeComponent>
            <ion-toolbar>
                <ion-buttons slot="start">
                    <ion-button (click)="onClickFilter()">
                        <ion-icon slot="icon-only" name="funnel"></ion-icon>
                    </ion-button>
                </ion-buttons>
                <ion-buttons slot="end" *ngIf="addable || cancelable">
                    <ion-button *ngIf="addable" (click)="onClickAdd()">
                        <ion-icon slot="icon-only" name="add"></ion-icon>
                    </ion-button>
                    <ion-button *ngIf="cancelable" (click)="onClickCancel()">
                        <ion-icon slot="icon-only" name="close-circle-outline"></ion-icon>
                    </ion-button>
                </ion-buttons>
               <ion-text class="text-muted">Total filter count: {{connector.countingSubject | async}}</ion-text>
            </ion-toolbar> 
        </ng-template>
  `,
})
export class FCFilterToolbarComponent {

    @Input() connector: FilterConnectorService;
    @ContentChild(FCFilterCompositeDirective, { static: true }) filterCompositeComponent: FCFilterCompositeDirective;

    @Input() addable: boolean = false;
    @Output() add = new EventEmitter();

    @Input() cancelable: boolean = false;
    @Output() cancel = new EventEmitter();

    @Input() visible: boolean = true;

    constructor(private _modalController: ModalController) { }

    async onClickFilter() {

        let modal = await this._modalController.create({
            component: FCFilterModalComponent,
            componentProps: {
                components: this.filterCompositeComponent.filterItemsComponent,
                connector: this.connector,
                title: this.filterCompositeComponent.title
            }
        });

        modal.onWillDismiss().then((a) => {
            if (a.data) {
                this.connector.next(a.data);
            }
        });

        return await modal.present();
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
