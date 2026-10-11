import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { TNCTransactionTypeService } from 'src/app/services/tnc-transaction-type.service';
import { TNCPropertyTypeService } from 'src/app/services/tnc-property-type.service';
import { TNCPropertyPurposeService } from 'src/app/services/tnc-property-purpose';
import { TNCInterestRateService } from 'src/app/services/tnc-intereset-rate.service';
import { TNC_DurationService } from 'src/app/services/tnc_duration.service';
import { TNC_TimeFrameService } from 'src/app/services/tnc_timeFrame.service';
import { map } from 'rxjs/operators';
import { PageUtilityService } from './page-utility.service';
import { MarketingItemService } from 'src/app/services/marketing-item.service';
import { TNC_RateTypeService } from 'src/app/services/tnc-rateType.service';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-tnc',
    template: `
        <div [formGroup]="form">
            <ion-list>
                <ion-item>
                    <ion-label position="stacked">Transaction Type</ion-label>
                    <ng-container *ngIf="(_transactionTypeService | async) as items">
                        <ion-select formControlName="TransactionTypeSK">
                            <ion-select-option *ngFor="let p of items" 
                            [value]='p.SysKey'>{{p.Description}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item *ngIf="form?.get('TransactionTypeSK')?.value == '2017042514462609135'">
                    <ion-label position="stacked">Construction Status</ion-label>
                    <ion-select formControlName="TransactionTypeCompleteStatus">
                        <ion-select-option *ngFor="let p of [{value:true,text:'COMPLETED'},{value:false,text:'UN-COMPLETED'}]" 
                        [value]='p.value'>{{p.text}}</ion-select-option>
                    </ion-select>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Property Type</ion-label>
                    <ng-container *ngIf="(_propertyTypeService | async) as items">
                        <ion-select formControlName="PropertyTypeSK">
                            <ion-select-option *ngFor="let p of items" [value]='p.SysKey'>{{p.Description}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Property Purpose</ion-label>
                    <ng-container *ngIf="(_propertyPurposeService | async) as items">
                        <ion-select formControlName="PropertyPurposeSK">
                            <ion-select-option *ngFor="let p of items" [value]='p.SysKey'>{{p.Description}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item-divider color="primary">
                    <ion-label>
                        Loan Terms
                    </ion-label>
                </ion-item-divider>
                <ion-item>
                    <ion-label position="stacked">Interest Rate Type</ion-label>
                    <ng-container *ngIf="(_propertyInterestRateService | async) as items">
                        <ion-select formControlName="InterestRateTypeSK">
                            <ion-select-option *ngFor="let p of items" [value]='p.SysKey'>{{p.Description}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Duration</ion-label>
                    <ng-container *ngIf="(_propertyDurationService | async) as items">
                        <ion-select formControlName="InterestRateTypeDurationSK">
                            <ion-select-option *ngFor="let p of items" [value]='p.SysKey'>{{p.Description}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Rate Type</ion-label>
                    <ng-container *ngIf="(_rateTypeService | async) as items">
                        <ion-select formControlName="RateTypeSK">
                            <ion-select-option *ngFor="let p of items" [value]='p.SysKey'>{{p.Description}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Time Frame</ion-label>
                    <ng-container *ngIf="(_propertyTimeFrameService | async) as items">
                        <ion-select formControlName="RateTypeTimeFrameSK">
                            <ion-select-option *ngFor="let p of items" [value]='p.SysKey'>{{p.Description}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Rate/Spread/Margin (%)</ion-label>
                    <ion-input fcDecimal fraction="4" formControlName="RateTypeMargin"></ion-input>
                    <fc-validation-container [control]="form.get('RateTypeMargin')" label="Rate/Spread/Margin (%)">
                        <fc-validation-min message="should not be less than 0"></fc-validation-min>
                        <fc-validation-max message="should not be large than 13"></fc-validation-max>
                    </fc-validation-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Loan Amount</ion-label>
                    <ion-input fcCurrency formControlName="LoanAmount"></ion-input>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Loan Tenure (Years)</ion-label>
                    <ion-input type="number" formControlName="LoanTenure"></ion-input>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Lock-In Period (Months)</ion-label>
                    <ion-input type="number" formControlName="LookInPeriod"></ion-input>
                </ion-item>
                <ion-item-divider color="primary">
                    <ion-label>
                        Marketing Promotion
                    </ion-label>
                </ion-item-divider>
                <ng-container *ngIf="marketingItems.length > 0;else marketingItemEmptyTemplate">
                    <ng-container *ngTemplateOutlet="marketingItemTemplate"></ng-container>
                </ng-container>
                <ng-template #marketingItemTemplate>
                    <ion-card *ngFor="let data of marketingItems">
                        <ion-card-content>
                            <ion-item lines="none">
                                <ion-label>{{ data.ItemDescription }}</ion-label>
                                {{ data.Amount | currency }}
                            </ion-item>
                        </ion-card-content>
                    </ion-card>
                </ng-template>
                <ng-template #marketingItemEmptyTemplate>
                    <ion-grid style="height: 80px">
                        <ion-row justify-content-center align-items-center style="height: 100%">
                            <ion-text class="text-muted">
                                No Records Found.
                            </ion-text>    
                        </ion-row>
                    </ion-grid>
                </ng-template>
                <ion-item-divider color="primary">
                    <ion-label>
                        Others
                    </ion-label>
                </ion-item-divider>
                <ion-item>
                    <ion-label>Legal Subsidy</ion-label>
                    <ion-toggle formControlName="LegalSubsidy"></ion-toggle>
                </ion-item>
                <ion-item>
                    <ion-label>Valuation Subsidy</ion-label>
                    <ion-toggle formControlName="ValuationSubsidy"></ion-toggle>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Other Promotion</ion-label>
                    <ion-textarea rows="3" formControlName="OtherPromotion"></ion-textarea>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Remarks</ion-label>
                    <ion-textarea rows="3" formControlName="Remark"></ion-textarea>
                </ion-item>
            </ion-list>
        </div>
    `,
    providers: [TNCTransactionTypeService,
        TNCPropertyTypeService,
        TNCPropertyPurposeService,
        TNCInterestRateService,
        TNC_DurationService,
        MarketingItemService,
        TNC_RateTypeService,
        TNC_TimeFrameService],
})
export class TNCPage implements OnInit, OnDestroy {

    @Input() form: FormGroup;
    private _subsink = new SubSink();

    marketingItems = [];

    constructor(public _transactionTypeService: TNCTransactionTypeService,
        public _propertyTypeService: TNCPropertyTypeService,
        public _propertyPurposeService: TNCPropertyPurposeService,
        public _propertyInterestRateService: TNCInterestRateService,
        public _marketingItemService: MarketingItemService,
        public _propertyDurationService: TNC_DurationService,
        public _pageUtilityService: PageUtilityService,
        public _rateTypeService: TNC_RateTypeService,
        public _propertyTimeFrameService: TNC_TimeFrameService) {

        this._transactionTypeService.asyncFinds();
        this._propertyTypeService.asyncFinds();
        this._propertyPurposeService.asyncFinds();
        this._propertyInterestRateService.asyncFinds();
        this._propertyDurationService.asyncFinds();
        this._propertyTimeFrameService.asyncFinds();
        this._rateTypeService.asyncFinds();

        this._subsink.sink = this._pageUtilityService.marketingItemSubject
            .pipe(map((response: any[]) => {
                response.map((data) => {
                    this._subsink.sink = this._marketingItemService.findByKey(data.ItemSK)
                        .subscribe((item) => {
                            data['ItemDescription'] = item.Description;
                        })
                })
                return response;
            }))
            .subscribe((data) => this.marketingItems = data);
    }

    ngOnInit() {

        this.disableTransactionType(this.form.get('TransactionTypeSK').value);
        this._subsink.sink = this.form.get('TransactionTypeSK').valueChanges
            .subscribe((data) => {
                this.disableTransactionType(data);
            });

        this.form.get('RateTypeMargin').setValidators([Validators.min(0), Validators.max(13)]);
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    async getItemDescription(key: string) {

        let data = await this._marketingItemService.findByKey(key).toPromise();
        return data.Description;
    }

    private disableTransactionType(key: string) {

        if (key == '2017042514462609136')
            this.form.get('TransactionTypeCompleteStatus').setValue(false);
    }
}