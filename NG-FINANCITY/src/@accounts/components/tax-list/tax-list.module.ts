import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaxListComponent } from './tax-list.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { TaxDetailModule } from '../tax-detail/tax-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [TaxListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    TaxDetailModule,
    RouterModule.forChild([{ path: '', component: TaxListComponent }])
  ]
})
export class TaxListModule { }
