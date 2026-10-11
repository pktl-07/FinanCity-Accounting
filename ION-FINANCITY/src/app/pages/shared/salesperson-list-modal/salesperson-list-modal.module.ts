import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { SalespersonListModalPage } from './salesperson-list-modal.page';

@NgModule({
  entryComponents: [SalespersonListModalPage],
  declarations: [SalespersonListModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCPagerMoudle,
    FCFilterModule,
    FCFilterToolbarModule
  ],
  exports: [SalespersonListModalPage]
})
export class SalespersonListModalModule { }
