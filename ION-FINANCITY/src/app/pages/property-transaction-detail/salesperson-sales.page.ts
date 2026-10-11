import { Component, OnInit, OnDestroy } from '@angular/core';
import { map } from 'rxjs/operators';
import { ModalController } from '@ionic/angular';
import { FormBuilder } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { PageUtilityService } from './page-utility.service';
import { SalespersonSalesModalPage } from '../shared/salesperson-sales-modal/salesperson-sales-modal.page';
import { CommissionStructureService } from 'src/app/services/commission-structure.service';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-property-transaction-salesperson-sales',
    template: `
        <ion-toolbar>
            <ion-title><h6>Salesperson</h6></ion-title>
            <ion-buttons slot="end">
                <ion-button (click)="handleSelect()">
                    <ion-icon slot="icon-only" name="add"></ion-icon>
                </ion-button>
            </ion-buttons>
        </ion-toolbar>
        <ion-content slot="fixed">
            <ion-card *ngFor="let data of _pageUtilityService.internalSalespersonSubject | async">
                <ion-item>
                    <ion-checkbox [disabled]="true" slot="start" [(ngModel)]="data.IsMain"></ion-checkbox>
                    <ion-label>{{ data.SalespersonName }}</ion-label>
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
                    <ion-button [disabled]="data.IsMain" size="default" color="danger" fill="clear" slot="start" (click)="handleDelete(data)">
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
    providers: [SalespersonManagementService,
        CommissionStructureService],
})
export class SalespersonSalesPage implements OnInit, OnDestroy {

    private _subsink = new SubSink();

    constructor(private _salespersonManagementService: SalespersonManagementService,
        public _pageUtilityService: PageUtilityService,
        private _modalController: ModalController,
        private _commissionStructureDetailService: CommissionStructureService,
        private _formBuilder: FormBuilder,
        private _formUtilityService: FormUtilityService,) { }

    ngOnInit(): void {

        this._subsink.sink = this._pageUtilityService.internalSalespersonSubject.pipe(
            map((response: any[]) => {
                response.map(d => {
                    let gross = this._pageUtilityService.grossCommissionAmountSubject.value;
                    this._subsink.sink = this._salespersonManagementService.findByKey(d.SPSK)
                        .subscribe((data) => {
                            d.SharePercent = gross > 0 ? (d.GrossComm / gross) * 100 : 0;
                            d.SalespersonName = data.Person.Name;
                            d.VendorSK = '1';
                        })
                })
                return response;
            })
        ).subscribe();
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    handleDelete(data: any) {

        this._pageUtilityService.internalSalespersonSubject.value.splice(this._pageUtilityService.internalSalespersonSubject.value.indexOf(data), 1)
    }

    async handleSelect(data?: any) {

        if (data) {
            //for edit template
            let form = await this.createForm(data);
            const m = await this._modalController.create({
                component: SalespersonSalesModalPage,
                componentProps: {
                    form: form,
                    transactionAmout: this._pageUtilityService.grossCommissionAmountSubject.value,
                    disableSalespersonOption: data.IsMain,
                },
            })

            m.onWillDismiss().then((item) => {
                if (item.data) {
                    Object.assign(data, item.data);
                }
            })

            return await m.present();

        } else {

            //for new template
            let form = await this.createForm();
            const m = await this._modalController.create({
                component: SalespersonSalesModalPage,
                componentProps: {
                    form: form,
                    transactionAmout: this._pageUtilityService.grossCommissionAmountSubject.value,
                }
            })

            m.onWillDismiss().then((item: any) => {

                if (item.data) {

                    this._pageUtilityService.internalSalespersonSubject.value.push(item.data)
                }
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
                PTOSK: this._pageUtilityService.PTOSKSubject.value,
                TaxSK: this._pageUtilityService.taxSubject.value.key,
                TaxCode: this._pageUtilityService.taxSubject.value.code,
                TaxPercent: this._pageUtilityService.taxSubject.value.percent,
                VendorSK: '1',
                SchemeDetailSK: null,

            }, null, fg => { }, ['GrossComm', 'GSTValue', 'TotalPayable']);

            return this._formBuilder.group(c);
        }
    }
}
