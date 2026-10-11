import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { ModalController } from '@ionic/angular';
import { NotificationService } from 'src/app/services/notification.service';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-mortgage-sales-notification-list',
    template: `
    <ion-header translucent>
        <ion-toolbar>
            <ion-title>
                <h5>Mortgage Sales</h5>
            </ion-title>
            <ion-buttons slot="end">
                <ion-button (click)="handleClose()" color="primary">Close</ion-button>
            </ion-buttons>
        </ion-toolbar>
    </ion-header>
    <ion-content>
        <fc-pager-container [data]="_notificationService" [method]="'asyncMortgageSalesPager'" #pagerContainer 
            [connector]="_filterConnectorService">
            <ng-template fcCustomDataDirective let-dataItem="dataItem" let-index="index" let-context="context">
                <ion-card>
                    <ion-card-content>
                        <ion-item-sliding>
                            <ion-item button (click)="detail(dataItem?.RefSK)" lines="none">
                                <ion-avatar slot='start'>
                                    <ion-img [src]="(dataItem?.SenderSK | fcImage | async) || 'assets/icon/empty-picture.png'">
                                    </ion-img>
                                </ion-avatar>
                                <span class='text-muted'>{{ dataItem?.SenderSK | fcName | async}} has <b>{{ transalate(dataItem) }}</b>
                                  <!--  for <b>{{ dataItem?.MaillingClientSK | fcName | async }}</b> -->
                                    on <b>{{dataItem?.CreatedDate | fcDateFormat}}</b>.
                                </span>
                            </ion-item>
                            <ion-item-options side="end">
                                <ion-item-option color="danger" (click)="hide(dataItem,context)">Hide</ion-item-option>
                            </ion-item-options>
                        </ion-item-sliding>
                    </ion-card-content>
                </ion-card>
            </ng-template>
        </fc-pager-container>
    </ion-content>
    <ion-footer>
        <ion-toolbar>
            <ion-text class="text-muted ion-padding" slot="end">
                {{pagerContainer.recordInfoSubject | async}}
            </ion-text>
        </ion-toolbar>
    </ion-footer>
    `,
    providers: [
        NotificationService,
        FilterConnectorService
    ]
})
export class MorgtageSalesNotificationModalComponent implements OnDestroy {

    private _sussink = new SubSink();
    constructor(private _router: Router,
        public _modalController: ModalController,
        public _notificationService: NotificationService,
        public _filterConnectorService: FilterConnectorService,
    ) { }

    ngOnDestroy(): void {
        this._sussink.unsubscribe();
    }

    async handleClose() {
        await this._modalController.dismiss();
    }

    transalate(dataItem: any) {
        const description = (<string>dataItem.Description).split(',');
        return `${description[0]} ${description[1]}`;
    }

    hide(dataItem: any, context: BehaviorSubject<any[]>) {
        this._sussink.sink = this._notificationService.hide(dataItem.SysKey)
            .subscribe(() => {
                context.value.splice(context.value.indexOf(dataItem), 1)
            });
    }

    detail(key: string) {
        this._modalController
            .dismiss()
            .then(() => {
                this._router.navigateByUrl(`/property-transaction-detail/${key}`)
            })
    }
}
