import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommissionReceivedListComponent } from './commission-received-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CommissionReceivedListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PGridViewModule,
    RouterModule.forChild([{ path: '', component: CommissionReceivedListComponent }])
  ]
})
export class CommissionReceivedListModule { }
