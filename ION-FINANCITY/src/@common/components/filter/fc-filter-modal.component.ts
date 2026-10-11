import { Component, QueryList, Input } from '@angular/core';
import { FCFilterItemDirective } from './fc-filter-item.directive';
import { ModalController } from '@ionic/angular';
import { Observable } from 'rxjs';
import { FilterConnectorService } from '../validator/fc.filter-connector.service';

@Component({
    selector: 'fc-filter-modal',
    template: `
    <ion-header translucent>
        <ion-toolbar>
            <ion-title><h5>{{ title }}</h5></ion-title>
            <ion-buttons slot="end">
                <ion-button (click)="onClickClose()" color="primary">Close</ion-button>
            </ion-buttons>
        </ion-toolbar>
    </ion-header>
    <ion-content>
        <ion-list padding>
            <ng-container *ngFor="let c of components">
                <ng-container *ngIf="c?.type == 'text'">
                    <ng-container *ngTemplateOutlet="inputTemplate;context:{ component: c }"></ng-container>
                </ng-container>
                <ng-container *ngIf="c?.type == 'dropdown'">
                    <ng-container *ngTemplateOutlet="dropDownTemplate;context:{ component: c }"></ng-container>
                </ng-container>
                <ng-container *ngIf="c?.type == 'date'">
                    <ng-container *ngTemplateOutlet="datePickerTemplate;context:{ component: c }"></ng-container>
                </ng-container>
                <ng-container *ngIf="c?.type == 'currency'">
                    <ng-container *ngTemplateOutlet="currencyTemplate;context:{ component: c }"></ng-container>
                </ng-container>
            </ng-container>
        </ion-list>
    </ion-content>
    <ion-footer>
        <ion-grid>
            <ion-row no-padding no-margin>
                <ion-col col-6 no-padding>
                    <ion-button expand="block" color="success" (click)="onClickOk()">
                        <ion-icon slot="icon-only" name="checkmark"></ion-icon>
                    </ion-button>
                </ion-col>
            </ion-row>
        </ion-grid>
    </ion-footer>
    
    <ng-template #inputTemplate let-component="component">
        <ion-item>
            <ion-label position="stacked">{{component.label}}</ion-label>
            <ion-input type="text" [(ngModel)]="component.value_1"></ion-input>
            <ion-buttons slot="end">
                <ng-container *ngIf="component.value_1 && component.value_1 != null">
                    <ng-container *ngTemplateOutlet="removeItemTemplate;context:{ component: component , target : 1 }"></ng-container>
                </ng-container>
            </ion-buttons>
        </ion-item>
    </ng-template>

    <ng-template #dropDownTemplate let-component="component">
        <ion-item>
            <ion-label position="stacked">{{component.label}}</ion-label>
            <ion-select multiple="true" [(ngModel)]="component.value_1">
                <ng-container *ngIf="isInstanceOfBehaviorSubject(component?.items)">
                    <ion-select-option *ngFor="let item of component?.items | async" [value]="item[component?.value]">
                    {{ item[component?.text] }}
                    </ion-select-option>
                </ng-container>
                <ng-container *ngIf="isInstanceOfBehaviorSubject(component?.items) == false">
                    <ion-select-option *ngFor="let item of component?.items" [value]="item[component?.value]">
                        {{ item[component?.text] }}
                    </ion-select-option>
                </ng-container>
            </ion-select>
            <ion-buttons slot="end">
                <ng-container *ngIf="(component.value_1 && component.value_1 != null) && component.value_1.length > 0 ">
                    <ng-container *ngTemplateOutlet="removeItemTemplate;context:{ component: component , target : 1  }"></ng-container>
                </ng-container>
            </ion-buttons>
        </ion-item>
    </ng-template>

    <ng-template #currencyTemplate let-component="component">
        <ion-item>
            <ion-label position="stacked">{{component.label}} <ion-text color="danger">(Greater Than)</ion-text></ion-label>
            <ion-input fcCurrency [ngModelOptions]="{updateOn : 'blur'}" nullable="true" [(ngModel)]="component.value_1"></ion-input>
            <ion-buttons slot="end">
                <ng-container *ngIf="component.value_1 && component.value_1 != null">
                    <ng-container *ngTemplateOutlet="removeItemTemplate;context:{ component: component , target : 1  }"></ng-container>
                </ng-container>
            </ion-buttons>
        </ion-item>
        <ion-item>
            <ion-label position="stacked">{{component.label}} <ion-text color="danger">(Less Than)</ion-text></ion-label>
            <ion-input fcCurrency [ngModelOptions]="{updateOn : 'blur'}" nullable="true" [(ngModel)]="component.value_2"></ion-input>
            <ion-buttons slot="end">
                <ng-container *ngIf="component.value_2 && component.value_2 != null">
                    <ng-container *ngTemplateOutlet="removeItemTemplate;context:{ component: component , target : 2  }"></ng-container>
                </ng-container>
            </ion-buttons>
        </ion-item>
    </ng-template>

    <ng-template #datePickerTemplate let-component="component">
        <ion-item>
            <ion-label position="stacked">{{component?.label}} <ion-text color="danger">(Greater Than)</ion-text></ion-label>
            <ion-datetime fcDate [(ngModel)]="component.value_1"></ion-datetime>
            <ion-buttons slot="end">
                <ng-container *ngIf="component.value_1 && component.value_1 != null">
                    <ng-container *ngTemplateOutlet="removeItemTemplate;context:{ component: component , target : 1 }"></ng-container>
                </ng-container>
            </ion-buttons>
        </ion-item>
        <ion-item>
            <ion-label position="stacked">{{component.label}} <ion-text color="danger">(Less Than)</ion-text></ion-label>
            <ion-datetime fcDate [(ngModel)]="component.value_2"></ion-datetime>
            <ion-buttons slot="end">
                <ng-container *ngIf="component.value_2 && component.value_2 != null">
                    <ng-container *ngTemplateOutlet="removeItemTemplate;context:{ component: component , target : 2 }"></ng-container>
                </ng-container>
            </ion-buttons>
        </ion-item>
    </ng-template>

    <ng-template #removeItemTemplate let-component="component" let-target="target">
        <ion-button slot="end" color="danger" (click)="onClearFiler(component,target)">
            <ion-icon name="close" size="large"></ion-icon>
        </ion-button>
    </ng-template>
    `,
    providers: []
})
export class FCFilterModalComponent {

    private _components: QueryList<FCFilterItemDirective>;
    public get components(): QueryList<FCFilterItemDirective> {
        return this._components;
    }
    public set components(value: QueryList<FCFilterItemDirective>) {
        this._components = value;
    }

    private _title: string;
    public get title(): string {
        return this._title;
    }
    public set title(value: string) {
        this._title = value;
    }

    @Input() connector: FilterConnectorService;

    constructor(private _modalController: ModalController) {
    }

    isInstanceOfBehaviorSubject(items: any) {
        return items instanceof Observable;
    }

    onClickClose() {
        this._modalController.dismiss();
    }

    onClickOk() {
        this._modalController.dismiss(this.connector.toKendoFilter(this._components));
    }

    onClearFiler(component: FCFilterItemDirective, target: number) {

        if (target == 1)
            component.value_1 = null;

        if (target == 2)
            component.value_2 = null;
    }
}
