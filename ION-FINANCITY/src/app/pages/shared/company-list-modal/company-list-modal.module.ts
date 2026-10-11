import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CompanyListModalPage } from './company-list-modal.page';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { CompanyCreateModalPageModule } from '../company-create-modal/company-create-modal.module';
import { FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  entryComponents: [CompanyListModalPage],
  declarations: [CompanyListModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCPagerMoudle,
    FCFilterModule,
    FCFilterToolbarModule,
    CompanyCreateModalPageModule,
    FCImagePipeModule
  ],
  exports: [CompanyListModalPage]
})
export class CompanyListModalPageModule { }
