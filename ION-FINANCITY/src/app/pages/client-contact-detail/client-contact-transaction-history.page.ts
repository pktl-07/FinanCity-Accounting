import { Component, Input, Inject, ViewChild } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PropertyTransactionService } from 'src/app/services/property-transaction.service';
import { IFCConfig } from 'src/@common/utils';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { FCPagerContainerComponent } from 'src/@common/components/pager/fc-pager-container-component';

@Component({
    selector: 'fc-client-contact-transaction-history',
    template: `
    <ion-header>
        <fc-filter-toolbar style="display: none;" [connector]="_filterConnectorService">
            <fc-filter-composite title="">
                <fc-filter-item field="ClientSK" label="" default="true" [value_1]="ClientSK" logic="and" type="text"></fc-filter-item>
                <fc-filter-item field="MainSalespersonSK" label="" default="true" [value_1]="MainSalespersonSK" logic="and" type="text"></fc-filter-item>
                <fc-filter-item field="InvoiceStatus" label="" default="true" [value_1]="8" logic="and" type="number" operator="eq"></fc-filter-item>
            </fc-filter-composite>
        </fc-filter-toolbar>
    </ion-header>
    <ion-content>
        <fc-pager-container [data]="_propertyTransactionService" #pagerContainer [connector]="_filterConnectorService">
            <fc-card>
                <fc-card-header field="InvoiceNo" icon="money"></fc-card-header>
                <fc-card-item>
                    <ng-template fcCardItemTemplate let-dataItem="dataItem">
                        <ion-item>
                            <ion-label position="stacked">Commission No.</ion-label>
                            {{ dataItem.SalesNo }}
                         </ion-item>
                        <ion-item>
                            <ion-label position="stacked">Transaction Date</ion-label>
                            {{ dataItem.TransactionDate | fcDateFormat }}
                        </ion-item>
                        <ion-item>
                            <ion-label>Property Address</ion-label>
                            {{ dataItem.PropertyAddress }}
                        </ion-item>
                        <ion-item>
                            <ion-label position="stacked">Loan Amount</ion-label>
                            {{ dataItem.LoanAmount | currency }}
                        </ion-item>
                        <ion-item lines="none">
                            <ion-label position="stacked">Bank</ion-label>
                            {{ dataItem.MortgageBank }}
                        </ion-item>
                    </ng-template>
                </fc-card-item>
            </fc-card>
        </fc-pager-container>
    </ion-content>
    <ion-footer>
        <ion-toolbar>
            <ion-text class="text-muted" slot="end">
                {{ pagerContainer.recordInfoSubject | async }}
            </ion-text>
        </ion-toolbar>
    </ion-footer>
    `,
    providers: [PropertyTransactionService,
        FilterConnectorService
    ]
})
export class ClientTransactionHistoryPage {

    @ViewChild('pagerContainer', { read: FCPagerContainerComponent, static: true }) pager: FCPagerContainerComponent;
    @Input() ClientSK: string;
    @Input() MainSalespersonSK: string = this._config.auth.SPSK;
    history = new BehaviorSubject([]);

    constructor(
        public _filterConnectorService: FilterConnectorService,
        public _propertyTransactionService: PropertyTransactionService,
        @Inject('fc.config') public _config: IFCConfig) { }

    public reset() {
        this.pager.clear();
    }
}
