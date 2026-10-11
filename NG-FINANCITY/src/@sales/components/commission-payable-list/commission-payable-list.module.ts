import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommissionPayableListComponent } from './commission-payable-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { CommissionPayableDetailListComponent } from './commission-payable-detail-list/commission-payable-detail-list.component';
import { RouterModule } from '@angular/router';

@NgModule({
  entryComponents: [CommissionPayableListComponent, CommissionPayableDetailListComponent],
  declarations: [CommissionPayableListComponent, CommissionPayableDetailListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PGridViewModule,
    RouterModule.forChild([{ path: '', component: CommissionPayableListComponent }])
  ]
})
export class CommissionReceivedAllocationListModule { }
