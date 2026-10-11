import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { GlobalConfigurationSubject } from 'src/app/mk-config';
import { NotificationService } from 'src/@sales/services/notification-service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';

@Component({
    selector: 'app-chatting-notification',
    template: `
        <p-notification-popup [icon]="'far fa-envelope'" [badgetCount]='_global?.ChattingCounter'
            (open)='handleChattingOpen()'>
            <ng-template fcNotificationContentTemplate>
                <p-notification-message-template [data]="chittingInfo | async">
                    <ng-template fcNotificationBodyTemplate let-dataItem="dataItem">
                        <div class='row'>
                            <div class="col-md-3">
                                <img class="img-thumbnail"
                                    src="{{ dataItem?.SenderSK | pSalespersonImage | async }}"
                                    onerror="this.src='assets/imgs/empty-picture.png'">
                            </div>
                            <div class="col-md-9">
                                <span class="text-muted">{{ dataItem?.SenderSK | pSalespersonName | async }}
                                    has send you a message for {{ dataItem?.RefSK | pSalespersonName | async }} on
                                    {{ dataItem?.CreatedDate | pDateformat }}.
                                </span>
                            </div>
                        </div>
                        <div class='row mt-3'>
                            <div class='col-md-12'>
                                <span class="text-muted">{{ dataItem?.Description }}</span>
                            </div>
                        </div>
                    </ng-template>
                    <ng-template fcNotificationActionTemplate let-dataItem="dataItem">
                        <button class="k-button k-flat" style="color:var(--danger)"
                            (click)='handleHide(dataItem?.SysKey)'>
                            <i class="fas fa-minus-circle"></i>&nbsp;Hide
                        </button>
                        <button class="k-button k-flat" style="color:var(--primary)"
                            (click)='handleOpenClient(dataItem?.RefSK)'>
                            <i class="fas fa-arrow-right"></i>&nbsp;Go to
                        </button>
                    </ng-template>
                </p-notification-message-template>
            </ng-template>
        </p-notification-popup>
  `,
    providers: [
        NotificationService,
        SalespersonManagementService
    ]
})
export class ChattingNotificationComponent implements OnDestroy {

    private _subsink = new SubSink();
    constructor(
        public _router: Router,
        private _peopleService: SalespersonManagementService,
        public _global: GlobalConfigurationSubject,
        private _notification: NotificationService) { }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    handleOpenClient(key: string) {

        this._subsink.sink = this._peopleService.everyBody({
            filter: {
                logic: 'and',
                filters: kendoFilter([{ field: 'SysKey', value: key, operator: 'eq', logic: 'and' }])
            }
        }).subscribe((data: Array<any>) => {

            const transType = data[0]['TransType'];

            if (transType == 9)
                this._router.navigate([`Management/LeadManagement`]);
            else if (transType == 10)
                this._router.navigate([`Management/ClientManagement`]);

        })
    }

    chittingInfo = new Subject();
    handleChattingOpen() {

        this._subsink.sink = this._notification.getChatting()
            .subscribe((data: []) => {

                this.chittingInfo.next(data);
            })
    }

    handleHide(key: string) {

        this._subsink.sink = this._notification.hide(key)
            .subscribe();
    }
}
