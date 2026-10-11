import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { ModalController } from '@ionic/angular';
import { NotificationService } from 'src/app/services/notification.service';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { kendoFilter, SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-lead-followup-notification-list',
    template: `
    <ion-header translucent>
        <ion-toolbar>
            <ion-title>
                <h5>Follow up</h5>
            </ion-title>
            <ion-buttons slot="end">
                <ion-button (click)="handleClose()" color="primary">Close</ion-button>
            </ion-buttons>
        </ion-toolbar>
    </ion-header>
    <ion-content>
        <fc-pager-container [data]="_notificationService" [method]="'asyncFollowUpPager'" #pagerContainer 
            [connector]="_filterConnectorService">
            <ng-template fcCustomDataDirective let-dataItem="dataItem" let-index="index" let-context="context">
                <ion-card>
                    <ion-card-header>
                        <ion-item-sliding>
                            <ion-item button (click)="detail(dataItem?.LeadSK)" lines="none">
                                <ion-avatar slot='start'>
                                    <ion-img [src]="('1' | fcImage | async) || 'assets/icon/empty-picture.png'">
                                    </ion-img>
                                </ion-avatar>
                                <span class='text-muted'>
                                        You need to follow up {{dataItem?.LeadName }}
                                        on <b>{{dataItem?.StartDate | fcDateFormat}}</b>
                                        at <b>{{dataItem?.StartTime | fcTimeFormat}}</b>.
                                </span>
                            </ion-item>
                            <ion-item-options side="end">
                                <ion-item-option color="danger" (click)="hide(dataItem,context)">Hide</ion-item-option>
                            </ion-item-options>
                        </ion-item-sliding>
                    </ion-card-header>
                    <ion-card-content>
                        <span class='text-muted'>{{ dataItem?.Description | titlecase }}</span>
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
        FilterConnectorService,
        SalespersonManagementService
    ]
})
export class FollowUpModalComponent implements OnDestroy {

    private _subsink = new SubSink();

    constructor(private _router: Router,
        public _peopleService: SalespersonManagementService,
        public _modalController: ModalController,
        public _notificationService: NotificationService,
        public _filterConnectorService: FilterConnectorService) { }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    async handleClose() {
        await this._modalController.dismiss();
    }

    hide(dataItem: any, context: BehaviorSubject<any[]>) {

        this._subsink.sink = this._notificationService.hide(dataItem.NotificationSK)
            .subscribe(() => {
                context.value.splice(context.value.indexOf(dataItem), 1);
                this._notificationService.subscribeLeadFollowUpCount();
            });
    }

    detail(key: string) {

        this._subsink.sink = this._peopleService.everyBody({
            filter: {
                logic: 'and',
                filters: kendoFilter([{ field: 'SysKey', value: key, operator: 'eq', logic: 'and' }])
            }
        }).subscribe(async (data: Array<any>) => {

            const transType = data[0]['TransType'];
            if (transType == 9)
                await this._router.navigate([`lead-contact-detail/${key}`])
            else if (transType == 10)
                await this._router.navigate([`client-contact-detail/${key}`])

            await this._modalController.dismiss();
        })
    }
}
