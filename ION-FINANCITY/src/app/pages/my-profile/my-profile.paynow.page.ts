import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
    selector: 'fc-paynow',
    template: `
    <div [formGroup]="form">
        <ion-list>
            <ion-item>
                <ion-label position="stacked">Mobile Number</ion-label>
                <ion-input formControlName="MobileNumber"></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">NRIC Number</ion-label>
                <ion-input formControlName="NRIC"></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">UEN Number</ion-label>
                <ion-input formControlName="UEN"></ion-input>
            </ion-item>
        </ion-list>
    </div>
    `
})
export class PayNowPage {

    @Input() form: FormGroup;
}
