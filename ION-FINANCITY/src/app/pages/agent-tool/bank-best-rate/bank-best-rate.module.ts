import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { BankBestRatePage } from './bank-best-rate.page';
import { FCPagerMoudle } from 'src/@common/components/pager/fc-pager.module';
import { FCFilterModule } from 'src/@common/components/filter/fc-filter.module';
import { FCFilterToolbarModule } from 'src/@common/components/toolbar/fc-filter-toolbar.module';
import { FCFileOpenerModule } from 'src/@common/components/file-opener/fc-file-opener.module';
import { FCDateFormatPipeModule, FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  declarations: [BankBestRatePage],
  imports: [
    CommonModule,
    IonicModule,
    FCPagerMoudle,
    FCFilterModule,
    FCFilterToolbarModule,
    FCFileOpenerModule,
    FCDateFormatPipeModule,
    FCImagePipeModule
  ],
  exports: [BankBestRatePage]
})
export class BankBestRatePageModule { }
