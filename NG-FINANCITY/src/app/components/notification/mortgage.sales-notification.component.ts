import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { GlobalConfigurationSubject } from 'src/app/mk-config';
import { NotificationService } from 'src/@sales/services/notification-service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
    selector: 'app-mortgage-sales-notification',
    template: `
            <p-notification-popup [icon]="'far fa-exchange'" [badgetCount]='_global?.MortgageSalesCounter' [mr]="'10'"
                (open)='handleMortgageSalesOpen()'>
                <ng-template fcNotificationContentTemplate>
                    <p-notification-message-template [data]="mortgageSalesInfo | async">
                        <ng-template fcNotificationBodyTemplate let-dataItem="dataItem">
                            <div class="row">
                                <div class="col-md-3">
                                    <img class="img-thumbnail"
                                        src="{{ dataItem?.SenderSK | pSalespersonImage | async }}"
                                        onerror="this.src='assets/imgs/empty-picture.png'">
                                </div>
                                <div class="col-md-9">
                                    <span class="text-muted">{{dataItem?.SenderSK | pSalespersonName | async }}
                                        has submitted a case for
                                        {{ dataItem?.MaillingClientSK | pSalespersonName | async }} on
                                        {{ dataItem?.CreatedDate | pDateformat }}.
                                    </span>
                                </div>
                            </div>
                        </ng-template>
                        <ng-template fcNotificationActionTemplate let-dataItem="dataItem">
                            <button class="k-button k-flat hide-message" style="color:var(--danger)"
                                (click)='handleHide(dataItem?.SysKey)'>
                                <i class="fas fa-minus-circle"></i>&nbsp;Hide
                            </button>
                            <button class="k-button k-flat hide-message" style="color:var(--primary)"
                                (click)='handleOpenSales(dataItem?.RefSK)'>
                                <i class="fas fa-arrow-right"></i>&nbsp;Go to
                            </button>
                        </ng-template>
                    </p-notification-message-template>
                </ng-template>
            </p-notification-popup>
  `,
    providers: [
        NotificationService
    ]
})
export class MortgageSalesNotificationComponent implements OnDestroy {

    private _subsink = new SubSink();
    constructor(
        public _router: Router,
        public _global: GlobalConfigurationSubject,
        private _notification: NotificationService) { }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    mortgageSalesInfo = new Subject();
    handleMortgageSalesOpen() {

        this._subsink.sink = this._notification.getMortgageSales()
            .subscribe((data: []) => {
                this.mortgageSalesInfo.next(data);
            })
    }

    handleOpenSales(key: string) {

        this._router.navigate([`Business/PropertyTransactionDetail/${key}`]);
    }

    handleHide(key: string) {

        this._subsink.sink = this._notification.hide(key)
            .subscribe();
    }
}
