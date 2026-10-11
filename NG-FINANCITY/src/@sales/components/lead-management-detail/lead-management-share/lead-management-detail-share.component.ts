import { Component, OnDestroy, Input } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { LeadManagementDetailShareService } from './lead-management-detail-share.service';

@Component({
    selector: 'app-lead-management-detail-share',
    template: `
        <kendo-grid
            (selectionChange)="handleSelectionChange()"
            [kendoGridBinding]="_componentService?.originSubject | async"
            [style.height.%]="100"
            kendoGridSelectBy="SysKey"
            [selectedKeys]="keySelection">
            <ng-template kendoGridToolbarTemplate>
                <div class='input-group col-md-12'>
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fal fa-search"></i></span>
                    </div>
                    <input class='form-control' placeholder="Saearch By Salesperson Name" kendoTextBox (input)="handleFilter($event.target.value)"/>
                </div>
            </ng-template>
            <kendo-grid-checkbox-column
                [width]="50"
                [headerClass]="{'text-center': true}"
                [class]="{'text-center': true}"
                [resizable]="false"
                [columnMenu]="false"
                showSelectAll="true">
            </kendo-grid-checkbox-column>
            <kendo-grid-column field="SysKey" title="" [width]="85">
                <ng-template kendoGridCellTemplate let-dataItem>
                    <img [src]='dataItem?.SysKey | pSalespersonImage | async' 
                    class="fc-message-body fc-rounded-photo" onerror="this.src='assets/imgs/empty-picture.png'">
                </ng-template>
            </kendo-grid-column>
            <kendo-grid-column field="Name" title="Salesperson Name"></kendo-grid-column>
            <kendo-grid-column field="IsActive" title="Is Active" [headerClass]="{'text-center':true}"
                [class]="{'text-center':true}" media="sm" width="80">
                <ng-template kendoGridCellTemplate let-dataItem>
                    <ng-container *ngIf="dataItem?.IsActive == true;else falseTemplate">
                        <span class="text-success">
                            <i class="fas fa-check"></i>
                        </span>
                    </ng-container>
                    <ng-template #falseTemplate>
                        <span class="text-danger">
                            <i class="fas fa-times"></i>
                        </span>
                    </ng-template>
                </ng-template>
            </kendo-grid-column>
        </kendo-grid>
    `,
    providers: [
        LeadManagementDetailShareService
    ],
    styles: [`
        tr.text-center {
            text-align:center
        }
    `]
})
export class LeadManagementDetailShareComponent implements OnDestroy {

    @Input() originKeys = [];
    @Input() keySelection = [];
    @Input() insertedItems = new BehaviorSubject([]);
    @Input() deletedItems = new BehaviorSubject([]);
    private _timeout = [];

    constructor(public _componentService: LeadManagementDetailShareService) {
    }

    ngOnDestroy(): void {
        this._timeout.forEach((timeout) => clearTimeout(timeout))
    }

    handleFilter(value: string) {

        if (value != '')
            this._componentService.originSubject.next(this._componentService.originSubject.value
                .filter(f => (<string>f.Name)
                    .toLocaleLowerCase()
                    .includes(value.toLocaleLowerCase())));
        else
            this._componentService.originSubject.next(this._componentService.salespersonClone)
    }

    handleSelectionChange() {
        //should put timeout value because kendo selection change event fire before cell value has changed.
        const timeout = setTimeout(() => {
            const inserted = this._componentService.getInsertedItem({
                originKeys: this.originKeys,
                selectedKeys: this.keySelection
            });
            const deleted = this._componentService.getDeletedItem({
                originKeys: this.originKeys,
                selectedKeys: this.keySelection
            })
            this.insertedItems.next(inserted);
            this.deletedItems.next(deleted);
        }, (500));
        this._timeout.push(timeout);
    }
}
