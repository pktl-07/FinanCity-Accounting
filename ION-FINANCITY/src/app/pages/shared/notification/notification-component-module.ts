import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { MorgtageSalesNotificationModalComponent } from './mortgage.sales-notificatoin-modal.component';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCImageLoaderModule } from 'src/@common/components/image-loader/fc-image-loader.module';
import { ChattingNotificationModalComponent } from './chatting-notification-modal.component';
import { FCDateFormatPipeModule, FCImagePipeModule, FCNamePipeModule, FCTimeFormatPipeModule } from 'src/@common/pipes/fc.pipe.module';
import { NotificationSummaryModalComponent } from './notification-summary-modal.component';
import { NotificationSummaryComponent } from './notification-summary.component';
import { FollowUpModalComponent } from './follow-up-modal.component';

const components = [
    NotificationSummaryComponent,
    MorgtageSalesNotificationModalComponent,
    ChattingNotificationModalComponent,
    NotificationSummaryModalComponent,
    FollowUpModalComponent
];

const entry = [MorgtageSalesNotificationModalComponent,
    ChattingNotificationModalComponent,
    NotificationSummaryModalComponent,
    FollowUpModalComponent
];

@NgModule({
    entryComponents: [...entry],
    declarations: [...components],
    imports: [
        CommonModule,
        IonicModule,
        FCPagerMoudle,
        FCFilterModule,
        FCFilterToolbarModule,
        FCImageLoaderModule,
        FCImagePipeModule,
        FCDateFormatPipeModule,
        FCNamePipeModule,
        FCImagePipeModule,
        FCTimeFormatPipeModule
    ],
    exports: [...components]
})
export class FCNotificationComponentModule { }
