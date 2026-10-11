import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {  RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { MyCommissionListPage } from './my-commission-list.page';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCRecordStatusModule } from 'src/@common/components/status/fc-record-status.module';
import { FCNotificationComponentModule } from '../shared/notification/notification-component-module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    RouterModule.forChild([{ path: '', component: MyCommissionListPage }]),
    FCPagerMoudle,
    FCFilterToolbarModule,
    FCFilterModule,
    FCRecordStatusModule,
    FCNotificationComponentModule,
  ],
  declarations: [MyCommissionListPage],
  providers: []
})
export class MyCommissionListPageModule { }
