import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { VendorType } from 'src/@common/common.constant';
import { ModalController } from '@ionic/angular';
import { CompanyListModalPage } from '../shared/company-list-modal/company-list-modal.page';
import { ExtenralCobrokeListModalPage } from '../shared/external-cobroke-list-modal/external-cobroke-list-modal.page';
import { ExternalSalespersonService } from 'src/app/services/external-salesperson.service';
import { VendorService } from 'src/app/services/vendor.service';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-bill-to-cobroke',
    template: `
    <div [formGroup]="form">
       <ion-list>
            <ion-item>
                <ion-label>Bill to Vendor</ion-label>
                <ion-toggle formControlName="BillToCoBroke" (ionChange)="handleBillToClient()"></ion-toggle>
            </ion-item>
            <ion-item>
                <ion-label position="stacked">Vendor Type</ion-label>
                <ion-select formControlName="ClientTransType" [disabled]="!form.get('BillToCoBroke').value" (ionChange)="handelVendorType()">
                    <ion-select-option value='{{ null }}'>N/A</ion-select-option>
                    <ion-select-option *ngFor="let p of _vnedorType" [value]='p.value'>{{p.text}}</ion-select-option>
                </ion-select>
            </ion-item>
            <fc-modal-select [disabled]="!form?.get('BillToCoBroke')?.value || form?.get('ClientTransType')?.value == null"
                formControlName="CompanyName"
                label="Company"
                (click)="handleCompany()">
                <ng-template fcModalSelectContentTemplate>
                    <fc-validation-container [control]="form?.get('CompanyName')" label="Company">
                        <fc-validation-required></fc-validation-required>
                    </fc-validation-container>
                </ng-template>
            </fc-modal-select>
            <fc-modal-select #item 
                [disabled]="!form?.get('BillToCoBroke')?.value || form?.get('VendorSK')?.value == null"
                formControlName="ClientName"
                label="Contact Person" 
                (click)="item?.disabled ? null : handleBanker()">
                <ng-template fcModalSelectContentTemplate>
                    <fc-validation-container [control]="form?.get('ClientName')" label="Contact Person">
                        <fc-validation-required></fc-validation-required>
                    </fc-validation-container>
                </ng-template>
            </fc-modal-select>
       </ion-list>
    </div>
    `,
    providers: [ExternalSalespersonService, VendorService],
})
export class BillToCoBrokePage implements OnInit, OnDestroy {

    @Input() form: FormGroup;
    private _subsink = new SubSink();
    public _vnedorType = [];

    @Input() disabledFields: boolean = false;

    constructor(private _modalController: ModalController,
        private _vendorService: VendorService,
        private _externalSalespersonService: ExternalSalespersonService) { }

    ngOnInit(): void {

        this.form.addControl("CompanyName", new FormControl());
        this.form.addControl("VendorSK", new FormControl());
        this.form.addControl("ClientName", new FormControl());

        this._vnedorType = VendorType;

        //set CompanyName and VendorSK is depending on BankerSK 
        if (this.form.get('BankerSK').value != null) {
            this._subsink.sink = this._externalSalespersonService
                .findByKey(this.form.get('BankerSK').value)
                .subscribe((client) => {
                    this.form.get('ClientName').patchValue(client.Person.Name);
                    this._subsink.sink = this._vendorService.findByKey(client.Person.VendorSK)
                        .subscribe((vendor) => {
                            this.form.get('CompanyName').patchValue(vendor.Vendor.CompanyName);
                            this.form.get('VendorSK').patchValue(vendor.Vendor.SysKey);
                        })
                })
        }

        //set validation is depending on select bill to co-broke on or off.
        this._subsink.sink = this.form.get('BillToCoBroke').valueChanges
            .subscribe((data) => {
                if (data == true) {
                    this.form.get('CompanyName').setValidators([Validators.required]);
                    this.form.get('ClientName').setValidators([Validators.required]);
                } else {
                    this.form.get('CompanyName').clearValidators();
                    this.form.get('ClientName').clearValidators();
                }
            })
        this.form.get('CompanyName').setValidators([Validators.required]);
        this.form.get('ClientName').setValidators([Validators.required]);
    }

    async handleCompany() {

        let m = await this._modalController.create({
            component: CompanyListModalPage,
            componentProps: {
                transType: this.form.get('ClientTransType').value
            }
        });

        m.onWillDismiss().then((data) => {
            if (data.data) {
                this.form.get('ClientName').patchValue(null);
                this.form.get('BankerSK').patchValue(null);
                this.form.get('VendorSK').patchValue(data.data.SysKey);
                this.form.get('CompanyName').patchValue(data.data.CompanyName);
            }
            this.form.get('CompanyName').markAsTouched();
        })
        await m.present();
    }

    async handleBanker() {

        let m = await this._modalController.create({
            component: ExtenralCobrokeListModalPage,
            componentProps: {
                vendorSK: this.form.get('VendorSK').value,
                vendorType: this.form.get('ClientTransType').value,
                disabledCompanyOption: true
            }
        });

        m.onWillDismiss().then((data) => {

            if (data.data) {
                this.form.get('BankerSK').patchValue(data.data.SysKey);
                this.form.get('ClientName').patchValue(data.data.Name);
            }
            this.form.get('ClientName').markAsTouched();
        })

        await m.present();
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    handleBillToClient() {

        if (this.form.get('BillToCoBroke').value == false)
            this.form.get('ClientTransType').patchValue(null);
        else
            this.form.get('ClientTransType').patchValue(18);

        this.form.get('CompanyName').patchValue(null);
        this.form.get('ClientName').patchValue(null);
        this.form.get('VendorSK').patchValue(18);
        this.form.get('BankerSK').patchValue(null);
    }

    handelVendorType() {

        this.form.get('CompanyName').patchValue(null);
        this.form.get('ClientName').patchValue(null);
        this.form.get('VendorSK').patchValue(null);
        this.form.get('BankerSK').patchValue(null);
    }
}
