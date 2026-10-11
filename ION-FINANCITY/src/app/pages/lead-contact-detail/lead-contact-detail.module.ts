import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { LeadContactDetailPage } from './lead-contact-detail.page';
import { SuperTabsModule } from '@ionic-super-tabs/angular'
import { ContactPageModule } from '../shared/contact/contact.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { LeadPageModule } from '../shared/lead/lead.module';
import { LeadRemarkPageModule } from '../shared/lead-remarks/lead-remarks.module';
import { ShareContactsModule } from '../shared/share-contacts/share-contacts.module';
import { FCDateFormatPipeModule, FCTimeFormatPipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    SuperTabsModule,
    ContactPageModule,
    LeadPageModule,
    FCFilterModule,
    FCFilterToolbarModule,
    FCPagerMoudle,
    LeadRemarkPageModule,
    ShareContactsModule,
    FCDateFormatPipeModule,
    FCTimeFormatPipeModule,
    RouterModule.forChild([{ path: '', component: LeadContactDetailPage }]),
  ],
  declarations: [
    LeadContactDetailPage,
  ]
})
export class LeadContactDetailPageModule { }
