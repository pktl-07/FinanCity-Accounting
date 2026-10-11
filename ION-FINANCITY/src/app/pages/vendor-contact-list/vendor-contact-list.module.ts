import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { VendortListPage } from './vendor-contact-list.page';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCNotificationComponentModule } from '../shared/notification/notification-component-module';
import { FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

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
    RouterModule.forChild([{ path: '', component: VendortListPage }]),
    FCImagePipeModule
  ],
  declarations: [VendortListPage],
  providers: []
})
export class VendortListPageModule { }
