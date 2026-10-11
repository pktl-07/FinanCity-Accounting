import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { NameConventionService } from 'src/app/services/name-conventions.service';

@Component({
    selector: 'fc-my-profile-bank',
    template: `
    <div [formGroup]="form">
        <ion-list>
            <ion-item>
                <ion-label position="stacked">Bank Name</ion-label>
                <ion-input formControlName="BankName"></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Bank Branch Code</ion-label>
                <ion-input formControlName="BankBranchCode"></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Bank Account Name</ion-label>
                <ion-input formControlName="BankAccountName"></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Bank Account Number</ion-label>
                <ion-input formControlName="BankAccountNo"></ion-input>
            </ion-item>
          <!--  <ion-item>
                <ion-label>Giro</ion-label>
                <ion-toggle formControlName="HasGiro"></ion-toggle>
            </ion-item> -->
        </ion-list>
    </div>
    `,
    providers: [FormUtilityService, NameConventionService]
})
export class MyProfilBankPage {

    @Input() form: FormGroup;
}
