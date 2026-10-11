import { Component, ViewEncapsulation, Input, Output, EventEmitter, ContentChild } from '@angular/core';
import { NotificationBodyTemplateDirective } from './p-notification-body-template.directive';
import { NotificationHeaderTemplateDirective } from './p-notification-header-template.directive';
import { NotificationActionTemplateDirective } from './p-notification-action-template.directive';
import { ActionsLayout } from '@progress/kendo-angular-layout';

@Component({
    selector: 'p-notification-message-template',
    template: `
    <div *ngFor="let dataItem of data;let index = index">
        <span>
            <kendo-card [style.background-color]="index % 2 == 0 ? '#ffffff' : '#ede8e8'">
                <kendo-card-body>
                    <ng-template [ngIf]="!bodyTemplateDirective?.templateRef" 
                        [ngTemplateOutlet]="bodyTemplateDirective?.templateRef"
                        [ngTemplateOutletContext]="bodyTemplateContext(dataItem,index)">
                    </ng-template>
                </kendo-card-body>
                <kendo-card-actions [layout]="actionsLayout">
                    <ng-template [ngIf]="!actionTemplateDirective?.templateRef" 
                        [ngTemplateOutlet]="actionTemplateDirective?.templateRef"
                        [ngTemplateOutletContext]="actionTemplateContext(dataItem,index)">
                    </ng-template>
                </kendo-card-actions>
            </kendo-card>
        </span>
    </div>
  `,
    providers: [],
    styles: [`
  `],
    encapsulation: ViewEncapsulation.None
})
export class PNotificationMessageTemplateComponent {

    @Input() data: [];
    @Output() hide = new EventEmitter();
    @ContentChild(NotificationHeaderTemplateDirective, { static: true }) headerTemplateDirective: NotificationHeaderTemplateDirective;
    @ContentChild(NotificationBodyTemplateDirective, { static: true }) bodyTemplateDirective: NotificationBodyTemplateDirective;
    @ContentChild(NotificationActionTemplateDirective, { static: true }) actionTemplateDirective: NotificationActionTemplateDirective;
    
    public actionsLayout: ActionsLayout = 'start';
    
    constructor() { }


    private _headeTemplateContext: any = {};
    headerTemplateContext(item: any, index: number) {

        const context = this._headeTemplateContext;
        context.dataItem = item;
        context.index = index;
        return context;
    }

    private _bodyTemplateContext: any = {};
    bodyTemplateContext(item: any, index: number) {

        const context = this._bodyTemplateContext;
        context.dataItem = item;
        context.index = index;
        return context;
    }

    private _actionTemplateContext: any = {};
    actionTemplateContext(item: any, index: number) {

        const context = this._bodyTemplateContext;
        context.dataItem = item;
        context.index = index;
        return context;
    }

    hideContent(key: string) {

        this.hide.emit(key);
    }
}

