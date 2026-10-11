import { Component, OnInit, Input, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { TaxService } from 'src/app/services/tax.service';
import { getComissionContext, SubSink } from 'src/@common/utils';
import { PageUtilityService } from './page-utility.service';

@Component({
    selector: 'fc-commission',
    template: `
        <div [formGroup]="form">
            <ion-list>
                <ion-item>
                    <ion-label position="stacked">Transaction Price</ion-label>
                    <ion-input fcCurrency formControlName="TransactedPrice" 
                    (ionBlur)="handleTransactionPrice(form.get('TransactedPrice').value)"></ion-input>
                    <fc-validation-container [control]="form.get('TransactedPrice')" label="Transaction Price">
                        <fc-validation-required></fc-validation-required>
                        <fc-validation-min message="should not be less than 1"></fc-validation-min>
                    </fc-validation-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Commission Rate (%)</ion-label>
                    <ion-input type="number" formControlName="CommissionRate" (ionBlur)="handleCommissionRate(form.get('CommissionRate').value)" readonly></ion-input>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Gross Commission</ion-label>
                    <ion-input fcCurrency formControlName="GrossCommission" (ionBlur)="handleGrossCommission(form.get('GrossCommission').value)" readonly></ion-input>
                </ion-item>  
                <ion-item>
                    <ion-label position="stacked">GST</ion-label>
                    <ng-container *ngIf="(_taxService | async) as items">
                        <ion-select formControlName="TaxSK">
                            <ion-select-option value="{{ null }}">N/A</ion-select-option>
                            <ion-select-option *ngFor="let p of items" 
                            [value]='p.SysKey'>{{p.TaxCode}}</ion-select-option>
                        </ion-select>
                    </ng-container>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">GST (%)</ion-label>
                    <ion-input formControlName="TaxPercent" readonly readonly></ion-input>
                </ion-item>   
                <ion-item>
                    <ion-label position="stacked">GST ($)</ion-label>
                    <ion-input fcCurrency formControlName="TaxAmount" readonly></ion-input>
                </ion-item>
                <ion-item>
                    <ion-label position="stacked">Total Amount</ion-label>
                    <ion-input fcCurrency formControlName="TotalAmount" (ionBlur)="handleTotalAmount(form.get('TotalAmount').value)" readonly></ion-input>
                </ion-item>  
            </ion-list>
        </div>
    `,
    providers: [TaxService],
})
export class CommissionPage implements OnInit, OnDestroy {

    @Input() form: FormGroup;
    @Output() onChangeTransactionPrice = new EventEmitter();

    private _subsink = new SubSink();
    constructor(public _taxService: TaxService,
        private _pageUtilityService: PageUtilityService) {

        _taxService.asyncFinds();
    }

    ngOnInit() {

        this._subsink.sink = this.form.get('TaxSK').valueChanges
            .subscribe((key) => {

                if (key) {

                    this._subsink.sink = this._taxService.findByKey(key).subscribe((data) => {

                        this.form.get('TaxPercent').patchValue(data['TaxPercent']);
                        let gross = this.form.get('GrossCommission').value;

                        let context = getComissionContext({ tax: data['TaxPercent'], amount: gross, call: 'gross' });

                        this.form.get('TaxAmount').patchValue(context.taxAmount);
                        this.form.get('TotalAmount').patchValue(context.total);

                        this._pageUtilityService.taxSubject.next({
                            key: data['SysKey'],
                            code: data['TaxCode'],
                            percent: data['TaxPercent']
                        });
                    })
                } else {

                    this.form.get('TaxPercent').patchValue(0);
                    let gross = this.form.get('GrossCommission').value;

                    let context = getComissionContext({ tax: 0, amount: gross, call: 'gross' });

                    this.form.get('TaxAmount').patchValue(context.taxAmount);
                    this.form.get('TotalAmount').patchValue(context.total);

                    this._pageUtilityService.taxSubject.next({
                        key: null,
                        code: 'N/A',
                        percent: 0
                    });
                }
            })

        this._subsink.sink = this._taxService.findByKey(this.form.get('TaxSK').value)
            .subscribe((tax) => {
                if (tax) {
                    this._pageUtilityService.taxSubject.next({
                        code: tax.TaxCode,
                        key: tax.SysKey,
                        percent: tax.TaxPercent
                    })
                } else {
                    this._pageUtilityService.taxSubject.next({
                        code: 'N/A',
                        key: null,
                        percent: 0
                    })
                }
            });

        this._subsink.sink = this.form.get('GrossCommission').valueChanges
            .subscribe((price) => {
                this._pageUtilityService.grossCommissionAmountSubject.next(price);
            })

        this.form.get('TransactedPrice').setValidators([Validators.required, Validators.min(1)]);
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    handleTransactionPrice(value: any) {
        this.onChangeTransactionPrice.emit(value);
    }

    handleCommissionRate(rate: number) {

        let price = this.form.get('TransactedPrice').value;
        let tax = this.form.get('TaxPercent').value;
        let gross = (price / 100) * rate;
        let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

        this.form.get('GrossCommission').patchValue(gross);
        this.form.get('TaxAmount').patchValue(context.taxAmount);
        this.form.get('TotalAmount').patchValue(context.total);
    }

    handleGrossCommission(gross: number) {

        let price = this.form.get('TransactedPrice').value;
        let tax = this.form.get('TaxPercent').value;
        let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

        this.form.get('CommissionRate').patchValue((gross / price) * 100);
        this.form.get('TaxAmount').patchValue(context.taxAmount);
        this.form.get('TotalAmount').patchValue(context.total);
    }

    handleTotalAmount(total: number) {

        let price = this.form.get('TransactedPrice').value;
        let tax = this.form.get('TaxPercent').value;
        let context = getComissionContext({ tax: tax, amount: total, call: 'total' })

        this.form.get('CommissionRate').patchValue((context.gross / price) * 100);
        this.form.get('TaxAmount').patchValue(context.taxAmount);
        this.form.get('GrossCommission').patchValue(context.gross);
    }
}