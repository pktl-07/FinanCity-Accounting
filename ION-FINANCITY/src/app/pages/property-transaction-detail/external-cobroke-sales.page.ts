import { Component, OnInit, OnDestroy } from '@angular/core';
import { map } from 'rxjs/operators';
import { ModalController } from '@ionic/angular';
import { FormBuilder } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { PageUtilityService } from './page-utility.service';
import { ExternalSalespersonService } from 'src/app/services/external-salesperson.service';
import { ExternalSalesSalesModalPage } from '../shared/external-cobroke-sales-modal/external-cobroke-sales-modal.page';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-property-transaction-external-sales',
    template: `
        <ion-toolbar>
            <ion-title><h6>Co-Broke</h6></ion-title>
            <ion-buttons slot="end">
                <ion-button (click)="handleSelect()">
                    <ion-icon slot="icon-only" name="add"></ion-icon>
                </ion-button>
            </ion-buttons>
        </ion-toolbar>
        <ion-content slot="fixed">
            <ion-card *ngFor="let data of _pageUtilityService.externalSalespersonSubject | async">
                <ion-item>
                    <ion-avatar>
                        <ion-img [src]="(data?.VendorSK | fcImage:10 | async) || 'assets/icon/empty-picture.png'">
                        </ion-img>
                    </ion-avatar>
                    <ion-label>&nbsp;<span style="font-size: .9rem">{{ data?.SalespersonName }}</span></ion-label>
                </ion-item>
                <ion-card-content>
                    <ion-item lines="none">
                        <ion-label position="stacked">Share Percent</ion-label>
                        {{ data.SharePercent }}%
                    </ion-item>
                    <ion-item lines="none">
                        <ion-label position="stacked">Commission</ion-label>
                        {{ data.GrossComm | currency }}
                    </ion-item>
                </ion-card-content>
                <ion-item>
                    <ion-button size="default" color="danger" fill="clear" slot="start" (click)="handleDelete(data)">
                        Delete
                        <ion-icon name="trash"></ion-icon>
                    </ion-button>
                    <ion-button size="default" color="primary" fill="clear" slot="end"  (click)="handleSelect(data)">
                        View
                        <ion-icon name="checkmark"></ion-icon>
                    </ion-button>
                </ion-item>
            </ion-card>
        </ion-content>
    `,
    providers: [ExternalSalespersonService,
        FormUtilityService],
})
export class ExternalSalespersonSalesPage implements OnInit, OnDestroy {

    private _subsink = new SubSink();
    constructor(private _externalSalespersonSales: ExternalSalespersonService,
        public _pageUtilityService: PageUtilityService,
        private _modalController: ModalController,
        private _formBuilder: FormBuilder,
        private _formUtilityService: FormUtilityService) { }

    ngOnInit(): void {
        this._subsink.sink = this._pageUtilityService.externalSalespersonSubject.pipe(
            map((data) => {
                data.map(d => {
                    this._subsink.sink = this._externalSalespersonSales.findByKey(d.SPSK)
                        .subscribe((data) => {
                            let gross = this._pageUtilityService.grossCommissionAmountSubject.value;
                            d.SalespersonName = data.Person.Name;
                            d.SharePercent = gross > 0 ? (d.GrossComm / gross) * 100 : 0;
                            d.IsMain = false;
                            d.HasGST = d.TaxPercent > 0
                        })
                })
                return data;
            })
        ).subscribe();
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    handleDelete(data: any) {
        this._pageUtilityService.externalSalespersonSubject.value.splice(this._pageUtilityService.externalSalespersonSubject.value.indexOf(data), 1)
    }

    async handleSelect(data?: any) {
        if (data) {
            //for edit template
            let form = await this.createForm(data);
            const m = await this._modalController.create({
                component: ExternalSalesSalesModalPage,
                componentProps: {
                    form: form,
                    transactionAmout: this._pageUtilityService.grossCommissionAmountSubject.value,
                    disabledCompanyOption: false
                },
            })
            m.onWillDismiss().then((item) => {
                if (item.data)
                    Object.assign(data, item.data);
            })
            return await m.present();
        } else {
            //for new template
            let form = await this.createForm();
            const m = await this._modalController.create({
                component: ExternalSalesSalesModalPage,
                componentProps: {
                    form: form,
                    transactionAmout: this._pageUtilityService.grossCommissionAmountSubject.value
                }
            })
            m.onWillDismiss().then((item: any) => {
                if (item.data)
                    this._pageUtilityService.externalSalespersonSubject.value.push(item.data);
            })
            return await m.present();
        }
    }

    async createForm(data?: any) {

        if (data) {
            let c = this._formUtilityService.formGroupJson(data, null, fg => { }, ['GrossComm', 'GSTValue', 'TotalPayable'])
            let k = this._formBuilder.group(c);
            return k;
        } else {
            let c = this._formUtilityService.formGroupJson({
                SPSK: null,
                SalespersonName: null,
                SharePercent: 0,
                IsMain: false,
                GrossComm: 0,
                GSTValue: 0,
                TotalPayable: 0,
                TransType: 20,
                HasGST: false,
                PTOSK: this._pageUtilityService.PTOSKSubject.value,
                TaxSK: this._pageUtilityService.taxSubject.value.key,
                TaxCode: this._pageUtilityService.taxSubject.value.code,
                TaxPercent: this._pageUtilityService.taxSubject.value.percent,
                VendorSK: null,
            }, null, fg => { }, ['GrossComm', 'GSTValue', 'TotalPayable']);
            return this._formBuilder.group(c);
        }
    }
}
