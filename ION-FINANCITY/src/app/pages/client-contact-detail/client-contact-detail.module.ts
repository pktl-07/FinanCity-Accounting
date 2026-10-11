import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { ClientContactDetailPage } from './client-contact-detail.page';
import { SuperTabsModule } from '@ionic-super-tabs/angular'
import { ContactPageModule } from '../shared/contact/contact.module';
import { ClientPageModule } from '../shared/client/client.module';
import { ClientTransactionHistoryPage } from './client-contact-transaction-history.page';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCDateFormatPipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    RouterModule.forChild([{ path: '', component: ClientContactDetailPage }]),
    SuperTabsModule,
    ContactPageModule,
    ClientPageModule,
    FCFilterModule,
    FCFilterToolbarModule,
    FCPagerMoudle,
    FCDateFormatPipeModule
  ],
  declarations: [
    ClientContactDetailPage,
    ClientTransactionHistoryPage,
  ]
})
export class ClientContactDetailPageModule { }
