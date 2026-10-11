import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { CoBrokerContactListPage } from './cobroker-contact-list.page';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCNotificationComponentModule } from '../shared/notification/notification-component-module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ReactiveFormsModule,
    FCPagerMoudle,
    FCFilterModule,
    FCFilterToolbarModule,
    FCNotificationComponentModule,
    RouterModule.forChild([{ path: '', component: CoBrokerContactListPage }]),
  ],
  declarations: [CoBrokerContactListPage],
  providers: []
})
export class CoBrokerContactListPageModule { }
