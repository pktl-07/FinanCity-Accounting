import { Component, Input } from '@angular/core';
import { FormGroup, } from '@angular/forms';
import { UtilityService } from 'src/app/services/util.service';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { CommissionStructureService } from 'src/app/services/commission-structure.service';

@Component({
    selector: 'fc-my-profile-employment',
    template: `
    <div [formGroup]="historyForm">
        <ion-item>
            <ion-label position="stacked">Date Joined</ion-label>
            <ion-datetime fcDate formControlName="DateJoin" readonly></ion-datetime>
        </ion-item>
    </div>
    <div [formGroup]="form">
        <ion-list>
            <ion-item>
                <ion-label position="stacked">Close Sales Commission to Year</ion-label>
                <ion-input fcCurrency formControlName="CloseSalesCommissionToYear" readonly></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Close Sales Commission to Date</ion-label>
                <ion-input fcCurrency formControlName="CloseSalesCommissionToDate" readonly></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Close Sales Value to Year</ion-label>
                <ion-input fcCurrency formControlName="CloseSalesValueToYear" readonly></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Close Sales Value to Date</ion-label>
                <ion-input fcCurrency formControlName="CloseSalesValueToDate" readonly></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Director</ion-label>
                <ion-input formControlName="Director" readonly></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Manager</ion-label>
                <ion-input formControlName="Manager" readonly></ion-input>
            </ion-item>
          <!--  <ion-item>
                <ion-label position="stacked">Designation</ion-label>
                <ion-input formControlName="Designation" readonly></ion-input>
            </ion-item> -->
            <ion-item>
                <ion-label position="stacked">Commission Structure</ion-label>
                <ion-input formControlName="CommissionScheme" readonly></ion-input>
            </ion-item>
          <!--  <ion-item>
                <ion-label position="stacked">Salesperson Commission Pay-out(%)</ion-label>
                <ion-input formControlName="SalespersonPayout" readonly></ion-input>
            </ion-item> -->
        </ion-list>
    </div>
    `,
    providers: [
        UtilityService,
        SalespersonManagementService,
        CommissionStructureService
    ]
})
export class MyProfilEmploymentPage {

    @Input() historyForm: FormGroup;
    @Input() form: FormGroup;
}
