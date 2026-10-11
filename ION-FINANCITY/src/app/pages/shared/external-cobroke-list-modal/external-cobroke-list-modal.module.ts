import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { ExtenralCobrokeListModalPage } from './external-cobroke-list-modal.page';
import { ExternalCobrokeCreateModalPageModule } from '../external-cobroke-create-modal/external-cobroke-create-modal.module';
import { FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  entryComponents: [ExtenralCobrokeListModalPage],
  declarations: [ExtenralCobrokeListModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCPagerMoudle,
    FCFilterModule,
    FCFilterToolbarModule,
    ExternalCobrokeCreateModalPageModule,
    FCImagePipeModule
  ],
  exports: [ExtenralCobrokeListModalPage]
})
export class ExternalCobrokeListModalModule { }
