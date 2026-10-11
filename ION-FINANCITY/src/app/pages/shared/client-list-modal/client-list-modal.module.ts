import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ClientListModalPage } from './client-list-modal.page';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { ClientCreateModalPageModule } from '../client-create-modal/client-create-modal.module';
import { FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  entryComponents: [ClientListModalPage],
  declarations: [ClientListModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCPagerMoudle,
    FCFilterModule,
    FCFilterToolbarModule,
    ClientCreateModalPageModule,
    FCImagePipeModule
  ],
  exports: [ClientListModalPage]
})
export class ClientListModalModule { }
