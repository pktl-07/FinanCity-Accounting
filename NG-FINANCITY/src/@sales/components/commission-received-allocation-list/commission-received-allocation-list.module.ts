import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommissionReceivedAllocationListComponent } from './commission-received-allocation-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CommissionReceivedAllocationListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PGridViewModule,
    RouterModule.forChild([{ path: '', component: CommissionReceivedAllocationListComponent }])
  ]
})
export class CommissionReceivedAllocationListModule { }
