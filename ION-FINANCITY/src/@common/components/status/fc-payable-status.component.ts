import { Component, Input } from '@angular/core';

/**
 * @example
 *  <fc-record-status [status]="-3">
 * </fc-record-status>
 */
@Component({
    selector: 'fc-payable-status',
    template: `
    <ion-badge [color]="getColor(value)">
        <ng-container [ngSwitch]="value">
            <ng-container *ngSwitchCase="1">Pending</ng-container>
            <ng-container *ngSwitchCase="2">Paid to You</ng-container>
            <ng-container *ngSwitchDefault>
                Pending
            </ng-container>
        </ng-container>    
    </ion-badge>
  `,
})
export class FCPayableStatusComponent {

    @Input() value: number;

    constructor() { }

    getColor(dataItem: number) {

        switch (dataItem) {
            case 1: return "warning"
            case 2: return "success";
            default: return "warning";
        }
    }
}
