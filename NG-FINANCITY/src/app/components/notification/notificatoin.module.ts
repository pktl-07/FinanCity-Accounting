import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MortgageSalesNotificationComponent } from './mortgage.sales-notification.component';
import { PNotificationPopupModule } from 'src/@common/components/p-notification-popup/p-notification-popup.module';
import { PSalespersonPipeModule } from 'src/@common/pipe/p-salesperson-pipe/p-salesperson-pipe-module';
import { PDateFormatPipeModule } from 'src/@common/pipe/p-date-format-pipe/p-date-format-pipe-module';
import { ChattingNotificationComponent } from './chatting-notification-component';

const components = [MortgageSalesNotificationComponent,
    ChattingNotificationComponent
];

@NgModule({
    declarations: [...components],
    imports: [
        CommonModule,
        PNotificationPopupModule,
        PSalespersonPipeModule,
        PDateFormatPipeModule
    ],
    exports: [...components]
})
export class FCNotificationModule { }
