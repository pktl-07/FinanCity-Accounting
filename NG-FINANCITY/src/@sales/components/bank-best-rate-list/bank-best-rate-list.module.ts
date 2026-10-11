import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { BankBestRateListComponent } from './bank-best-rate-list.component';
import { BankBestRateDetailModule } from '../bank-best-rate-detail/bank-best-rate-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [BankBestRateListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    BankBestRateDetailModule,
    RouterModule.forChild([{ path: '', component: BankBestRateListComponent }])
  ]
})
export class BankBestRateListModule { }
