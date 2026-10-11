import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { PropertyTransactionListPage } from './property-transaction-list.page';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCRecordStatusModule } from 'src/@common/components/status/fc-record-status.module';
import { MortgageSalesTransactionListPage } from './mortgage-sales-transaction';
import { MortgageSalesFilterPage } from './mortgage-sales-filter';
import { ReferralTransactionListPage } from './referral-sales-transaction';
import { ReferralSalesFilterPage } from './referral-sales-filter';
import { FCNotificationComponentModule } from '../shared/notification/notification-component-module';
import { FCDateFormatPipeModule } from 'src/@common/pipes/fc.pipe.module';

const pages = [
  PropertyTransactionListPage,
  MortgageSalesTransactionListPage,
  MortgageSalesFilterPage,
  ReferralTransactionListPage,
  ReferralSalesFilterPage,
]

@NgModule({
  declarations: [...pages],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCPagerMoudle,
    FCFilterToolbarModule,
    FCFilterModule,
    FCRecordStatusModule,
    FCNotificationComponentModule,
    FCDateFormatPipeModule,
    RouterModule.forChild([{ path: '', component: PropertyTransactionListPage }]),
  ],
  providers: [],
  exports: [...pages]
})
export class PropertyTransactionListPageModule { }
