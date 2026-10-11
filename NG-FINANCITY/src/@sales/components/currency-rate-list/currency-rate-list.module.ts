import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { CurrencyRateListComponent } from './currency-rate-list.component';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { CurrencyRateDetailModule } from '../currency-rate-detail/currency-rate-detail.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CurrencyRateListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    CurrencyRateDetailModule,
    RouterModule.forChild([{ path: '', component: CurrencyRateListComponent }])
  ]
})
export class CurrencyRateListModule { }
