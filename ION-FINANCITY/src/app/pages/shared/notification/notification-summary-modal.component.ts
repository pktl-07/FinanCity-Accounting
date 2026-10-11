import { Component, Inject, OnDestroy } from '@angular/core';
import { NotificationService } from 'src/app/services/notification.service';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { IFCConfig, logoPicture, SubSink } from 'src/@common/utils';
import { ModalController } from '@ionic/angular';
import { ChattingNotificationModalComponent } from './chatting-notification-modal.component';
import { MorgtageSalesNotificationModalComponent } from './mortgage.sales-notificatoin-modal.component';
import { FollowUpModalComponent } from './follow-up-modal.component';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'fc-notification-modal',
    template: `
    <ion-content>
        <ion-list>
            <ion-item-divider>
                <ion-label>
                    Sales
                </ion-label>
            </ion-item-divider>
            <ion-item button detail="false" (click)="detail('mortagage')" lines="none">
                <ion-avatar>
                    <img [src]="getPicture('mortagage')">
                </ion-avatar>
                <span style="font-size: .9rem">&nbsp;Mortgage Sales</span>
                <ion-badge slot="end" color="danger">{{  config.general.mortageSalesCounter | async }}</ion-badge>
            </ion-item>
            <ion-item-divider>
                <ion-label>
                    Lead
                </ion-label>
            </ion-item-divider>
            <ion-item button detail="false" (click)="detail('chatting')" lines="none">
                <ion-avatar>
                    <img [src]="getPicture('chatting')">
                </ion-avatar>
                <span style="font-size: .9rem">&nbsp;Message</span>
                <ion-badge slot="end" color="danger">{{  config.general.chattingCounter | async }}</ion-badge>
            </ion-item>
            <ion-item button detail="false" (click)="detail('leadfollowup')" lines="none">
                <ion-avatar>
                    <img [src]="getPicture('leadfollowup')">
                </ion-avatar>
                <span style="font-size: .9rem">&nbsp;Follow up</span>
                <ion-badge slot="end" color="danger">{{  config.general.leadFollowUpCounter | async }}</ion-badge>
            </ion-item>
        </ion-list>       
    </ion-content>
    `,
    providers: [
        NotificationService,
        FilterConnectorService,
        SalespersonManagementService
    ]
})
export class NotificationSummaryModalComponent implements OnDestroy {

    private _subsink = new SubSink();

    public config: IFCConfig = environment;

    constructor(private _modalController: ModalController) {
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    async detail(category: 'mortagage' | 'chatting' | 'leadfollowup') {

        switch (category) {
            case 'mortagage':
                await this.openDetailModal(MorgtageSalesNotificationModalComponent)
                break;
            case 'chatting':
                await this.openDetailModal(ChattingNotificationModalComponent)
                break;
            case 'leadfollowup':
                await this.openDetailModal(FollowUpModalComponent)
                break;
        }
    }

    private async openDetailModal(compoent: any) {

        const m = await this._modalController.create({
            component: compoent,
        })
        await m.present();
    }

    async handleClose() {
        await this._modalController.dismiss();
    }

    getPicture(category: 'mortagage' | 'chatting' | 'leadfollowup') {

        switch (category) {
            case 'mortagage': return logoPicture['money'];
            case 'chatting': return logoPicture['envelop'];
            case 'leadfollowup': return logoPicture['thumb-up'];
            default: return null;
        }
    }
}
