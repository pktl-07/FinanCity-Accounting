import { Component, OnInit, ViewChild, Input, OnDestroy } from '@angular/core';
import { PropertyTransactionService } from 'src/app/services/property-transaction.service';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { FCPagerContainerComponent } from 'src/@common/components/pager/fc-pager-container-component';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-mortgage-sales-transaction-list',
    template: `
    <fc-pager-container [data]="_propertyTransactionService" #pagerContainer [connector]="connector">
        <fc-card (click)="onViewDetail($event)">
            <fc-card-header field="SalesNo" icon="money">
                <ng-template fcCardHeaderTemplate let-dataItem="dataItem">
                <fc-record-status [value]="dataItem.RecordStatus"></fc-record-status>
                </ng-template>
            </fc-card-header>
            <fc-card-item>
                <ng-template fcCardItemTemplate let-dataItem="dataItem">
                <ion-item lines="none">
                    <ion-text slot="start" class="text-muted">{{ dataItem.ClientName }}</ion-text>
                    <ion-text slot="end" class="text-muted">{{ dataItem.TransactionDate | fcDateFormat }}</ion-text>
                </ion-item>
                </ng-template>
            </fc-card-item>
        </fc-card>
    </fc-pager-container>
  `,
    providers: [
        PropertyTransactionService,
        FilterConnectorService]
})
export class MortgageSalesTransactionListPage implements OnInit, OnDestroy {

    @Input() connector: FilterConnectorService;
    @Input() recordInfo = new Subject();

    @ViewChild(FCPagerContainerComponent, { static: true }) private _pageContainer: FCPagerContainerComponent;

    private _subsink = new SubSink();

    readonly recordStatus = [
        { value: -3, text: 'Save Draft' },
        { value: -2, text: 'Reject' },
        { value: -1, text: 'Submit' },
        { value: 2, text: 'Enter Save' },
        { value: 3, text: 'Enter Update' },
        { value: 6, text: 'Void' },
        { value: 8, text: 'Post' }];

    constructor(public _propertyTransactionService: PropertyTransactionService,
        private _router: Router) {
    }

    ngOnInit() {

        this._subsink.sink = this._pageContainer.recordInfoSubject
            .subscribe((value) => {
                this.recordInfo.next(value);
            })
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    async onViewDetail(data: any) {

        return await this._router.navigateByUrl(`/property-transaction-detail/${data.PropertySK}`);
    }
}
