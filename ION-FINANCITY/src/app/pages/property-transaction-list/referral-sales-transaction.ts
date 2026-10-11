import { Component, OnInit, ViewChild, Input, OnDestroy } from '@angular/core';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { FCPagerContainerComponent } from 'src/@common/components/pager/fc-pager-container-component';
import { ReferralSalesService } from 'src/app/services/referral-sales-service';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-referral-sales-transaction-list',
    template: `
    <fc-pager-container [data]="_referralSalesService" #pagerContainer [connector]="connector">
        <fc-card>
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
        ReferralSalesService,
        FilterConnectorService]
})
export class ReferralTransactionListPage implements OnInit, OnDestroy {

    @Input() connector: FilterConnectorService;
    @Input() recordInfo = new Subject<any>();
    @ViewChild(FCPagerContainerComponent, { static: true }) private _pageContainer: FCPagerContainerComponent;

    private _subsink = new SubSink();

    readonly recordStatus = [
        { value: 2, text: 'Enter Save' },
        { value: 3, text: 'Enter Update' },
        { value: 6, text: 'Void' },
        { value: 8, text: 'Post' }];

    constructor(public _referralSalesService: ReferralSalesService,
        private _router: Router) {
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    ngOnInit() {

        this._subsink.sink = this._pageContainer.recordInfoSubject
            .subscribe((value) => {
                this.recordInfo.next(value);
            })
    }
}
