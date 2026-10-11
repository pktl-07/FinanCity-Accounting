import { Component, OnInit, AfterContentInit, ContentChild, Input, EventEmitter, Output } from '@angular/core';
import { FCFilterCompositeDirective } from '../filter/fc-filter-composite.directive';

/**
 * @example
 *  <fc-record-status [status]="-3">
 * </fc-record-status>
 */
@Component({
    selector: 'fc-record-status',
    template: `
    <ion-badge [color]="getColor(value)">
        <ng-container [ngSwitch]="value">
            <ng-container *ngSwitchCase="-10">New</ng-container>
            <ng-container *ngSwitchCase="-3">Save Draft</ng-container>
            <ng-container *ngSwitchCase="-2">Reject</ng-container>
            <ng-container *ngSwitchCase="-1">Submit</ng-container>
            <ng-container *ngSwitchCase="2">Pending</ng-container>
            <ng-container *ngSwitchCase="3">Pending</ng-container>
            <ng-container *ngSwitchCase="6">Void</ng-container>
            <ng-container *ngSwitchCase="8">Approve</ng-container>
        </ng-container>    
    </ion-badge>
  `,
})
export class FCRecordStatusComponent {

    @Input() value: number;

    constructor() { }

    getColor(dataItem: number) {

        switch (dataItem) {
            case -1: return "darkyellow"
            case -2: return "danger";
            case -3: return "tertiary";
            case -10: return "medium";
            case 2: return "warning";
            case 3: return "warning";
            case 6: return "danger";
            case 8: return "success";
        }
    }
}
