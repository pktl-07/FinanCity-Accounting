import { Component, OnInit, OnDestroy } from '@angular/core';
import { map } from 'rxjs/operators';
import { ClientService } from 'src/app/services/client-service';
import { nullDefault, SubSink } from 'src/@common/utils';
import { ModalController } from '@ionic/angular';
import { FormBuilder, FormControl } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { PageUtilityService } from './page-utility.service';
import { ClientListModalPage } from '../shared/client-list-modal/client-list-modal.page';
import { ClientCreateModalPage } from '../shared/client-create-modal/client-create-modal.page';

@Component({
    selector: 'fc-property-transaction-client',
    template: `
        <ion-toolbar>
            <ion-title><h6>Clients</h6></ion-title>
            <ion-buttons slot="end">
                <ion-button (click)="handleSelect()">
                    <ion-icon slot="icon-only" name="add"></ion-icon>
                </ion-button>
            </ion-buttons>
        </ion-toolbar>
        <ion-content slot="fixed">
            <ion-card *ngFor="let data of _pageUtilityService.clientSubject | async">
                <ion-item>
                    <ion-checkbox slot="start" [(ngModel)]="data.IsMain"></ion-checkbox>
                    <ion-avatar>
                        <ion-img [src]="(data?.ClientSK | fcImage:1 | async) || 'assets/icon/empty-picture.png'">
                        </ion-img>
                    </ion-avatar>
                    <ion-label>&nbsp;<span style="font-size: .9rem">{{ data?.ClientName }}</span></ion-label>
                </ion-item>
                <ion-card-content>
                    {{ data.Address }}
                </ion-card-content>
                <ion-item>
                    <ion-button size="default"  color="danger" fill="clear" slot="start" (click)="handleDelete(data)">
                        Delete
                        <ion-icon name="trash"></ion-icon>
                    </ion-button>
                    <ion-button size="default" color="primary" fill="clear" slot="end"  (click)="handleView(data)">
                        View
                        <ion-icon name="checkmark"></ion-icon>
                    </ion-button>
                </ion-item>
            </ion-card>
        </ion-content>
    `,
    providers: [ClientService, FormUtilityService],
})
export class ClientPage implements OnInit, OnDestroy {

    private _subsink = new SubSink();
    data: any;

    constructor(private _clientService: ClientService,
        public _pageUtilityService: PageUtilityService,
        private _modalController: ModalController,
        private _formBuilder: FormBuilder,
        private _formUtilityService: FormUtilityService) { }

    ngOnInit(): void {

        this._subsink.sink = this._pageUtilityService.clientSubject.pipe(
            map((data) => {
                data.map(d => {
                    this._subsink.sink = this._clientService.findByKey(d.ClientSK).
                        subscribe((data) => {
                            d.ClientName = data.Person.Name;
                            d.Address = `${nullDefault(data.Contact.BuildingHouseNo)} ${nullDefault(data.Contact.Address1)} ${nullDefault(data.Contact.Address2)} ${nullDefault(data.Contact.FloorUnit)} ${nullDefault(data.Contact.PostalCode)}`;
                        })
                })
                return data;
            })
        ).subscribe((s) => {
        })
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    handleDelete(data: any) {
        this._pageUtilityService.clientSubject.value.splice(this._pageUtilityService.clientSubject.value.indexOf(data), 1)
    }

    async handleView(data?: any) {
        //for edit template
        let form = await this.createForm(data.ClientSK);
        const m = await this._modalController.create({
            component: ClientCreateModalPage,
            componentProps: {
                form: form,
                action: 'edit',
                delete: () => this._pageUtilityService.clientSubject.value.splice(this._pageUtilityService.clientSubject.value.indexOf(data), 1),
            },
        })
        m.onWillDismiss().then((item: any) => {
            if (item.data) {
                data.ClientSK = item.data.Person.SysKey;
                data.ClientName = item.data.Person.Name;
                data.Address = `${nullDefault(item.data.Contact.BuildingHouseNo)}
                 ${nullDefault(item.data.Contact.Address1)} 
                 ${nullDefault(item.data.Contact.Address2)} 
                 ${nullDefault(item.data.Contact.FloorUnit)} 
                 ${nullDefault(item.data.Contact.PostalCode)}`;
                data.TransType = 10;
            }
        })
        await m.present();
    }

    async handleSelect() {
        const m = await this._modalController.create({
            component: ClientListModalPage,
        })
        m.onWillDismiss().then((data) => {
            this._pageUtilityService.clientSubject.value.push({
                IsMain: false,
                ClientSK: data.data.SysKey,
                ClientName: data.data.Name,
                Address: data.data.Address,
                TransType: 10,
                PTOSK: this._pageUtilityService.PTOSKSubject.value
            })
        });
        await m.present();
    }

    async createForm(key?: string) {
        let data = await this._clientService.findByKey(key).toPromise();
        let c = this._formUtilityService.formGroupJson(data, null, fg => {
            if (fg.name == "Person") {
                fg.form.addControl('FristName', new FormControl());
                fg.form.addControl('LastName', new FormControl());
            }
        })
        return this._formBuilder.group(c);
    }
}
