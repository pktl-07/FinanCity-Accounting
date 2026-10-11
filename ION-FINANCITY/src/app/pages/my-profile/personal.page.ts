import { Component, OnInit, Input, ViewChild, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { SalutationSource, GenderSource, MartialStatusSource } from 'src/@common/common.constant';
import { NameConventionService } from 'src/app/services/name-conventions.service';
import { FCImageLoaderPage } from 'src/@common/components/image-loader/fc-image-loader.page';
import { IFCConfig } from 'src/@common/utils';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'fc-my-profile-personal',
    template: `
    <div [formGroup]="form">
        <ion-list>
            <fc-image-loader [uploadId]="form?.get('SysKey')?.value" (onComplete)="handlePhotoCompleteTaken($event)"
                [fileType]="1">
            </fc-image-loader>
            <ion-item>
                <ion-label position="stacked">Salesperson No.</ion-label>
                <ion-input formControlName="R1" readonly></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Salutation</ion-label>
                <ion-select formControlName="Salutation">
                    <ion-select-option *ngFor="let p of salutation" [value]='p.value'>{{p.text}}</ion-select-option>
                </ion-select>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">First Name</ion-label>
                <ion-input formControlName="FirstName"></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Last Name</ion-label>
                <ion-input formControlName="LastName"></ion-input>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Display Name</ion-label>
                <ion-input formControlName="Name" readonly></ion-input>
                <fc-validation-container [control]="form.get('Name')" label="Display Name">
                    <fc-validation-required></fc-validation-required>
                </fc-validation-container>
            </ion-item>
            <div>
                <ion-radio-group formControlName="IsLastNameFirst">
                    <ion-list-header>
                        <ion-label>Name Options</ion-label>
                    </ion-list-header>
                    <ion-item>
                        <ion-label>'First Name' First</ion-label>
                    <ion-radio slot="start" [value]="false"></ion-radio>
                    </ion-item>
                    <ion-item>
                        <ion-label>'Last Name' First</ion-label>
                        <ion-radio slot="start" [value]="true"></ion-radio>
                    </ion-item>
                </ion-radio-group>
            </div>
            <ion-item>
                <ion-label position="stacked">NRIC/Passport No.</ion-label>
                <ion-input formControlName="NRIC"></ion-input>
                <fc-validation-container [control]="form.get('NRIC')" label="NRIC">
                    <fc-validation-required></fc-validation-required>
                </fc-validation-container>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Gender</ion-label>
                    <ion-select formControlName="Gender">
                        <ion-select-option *ngFor="let p of gender" [value]='p.value'>{{p.text}}</ion-select-option>
                    </ion-select>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Marital Status</ion-label>
                <ion-select formControlName="MartialStatus">
                    <ion-select-option *ngFor="let p of martialStatus" [value]='p.value'>{{p.text}}</ion-select-option>
                </ion-select>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Nationality</ion-label>
                    <ion-input formControlName="Nationality"></ion-input>
            </ion-item>
        </ion-list>
    </div>
    `,
    providers: [NameConventionService]
})
export class MyProfilePersonPage implements OnInit {

    salutation = [];
    gender = [];
    martialStatus = [];

    @Input() form: FormGroup;
    @ViewChild(FCImageLoaderPage, { static: true }) _imageLoader: FCImageLoaderPage;

    constructor(private _nameConventionService: NameConventionService) { }

    ngOnInit() {

        this.salutation = SalutationSource;
        this.gender = GenderSource;
        this.martialStatus = MartialStatusSource;

        this.form.get('Name').setValidators([Validators.required]);
        this.form.get('NRIC').setValidators([Validators.required]);

        this._nameConventionService.register({
            sourceName: this.form.get('Name'),
            fristName: this.form.get('FirstName'),
            lastName: this.form.get('LastName'),
            isLastNameFirst: this.form.get('IsLastNameFirst'),
        })

        this._nameConventionService.watch();
    }

    handlePhotoCompleteTaken(url: string) {

        environment.general.profilePhoto.next(url);
    }
}
