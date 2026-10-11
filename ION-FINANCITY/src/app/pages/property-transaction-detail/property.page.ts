import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { commissionStructureType, SubSink } from 'src/@common/utils';
import { BehaviorSubject } from 'rxjs';

@Component({
    selector: 'fc-property',
    template: `
    <div [formGroup]="form">
        <ion-list>
            <ion-item>
                <ion-label>Status</ion-label>
                <fc-record-status [value]="recordStatusSubject | async"></fc-record-status>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Transaction No.</ion-label>
                <ion-input formControlName="R1" readonly></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Transaction Date</ion-label>
                <ion-datetime fcDate formControlName="TransactionDate"></ion-datetime>
                <fc-validation-container [control]="form.get('TransactionDate')" label="Transaction Date">
                    <fc-validation-required></fc-validation-required>
                </fc-validation-container>
            </ion-item>
        </ion-list>
    </div>
    `,
    providers: [],
})
export class PropertyPage implements OnInit, OnDestroy {

    @Input() form: FormGroup;
    private _subsink = new SubSink();
    public recordStatusSubject = new BehaviorSubject(0);
    public _commissionStructureType = commissionStructureType();

    ngOnInit(): void {
        this._subsink.sink = this.form.get('RecordStatus')
            .valueChanges
            .subscribe((status) => {
                this.recordStatusSubject.next(status);
            })

        let status = this.form.get('RecordStatus').value == 0 ? -10
            : this.form.get('RecordStatus').value;

        this.recordStatusSubject.next(status);
        this.form.get('TransactionDate').setValidators([Validators.required]);
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }
}
