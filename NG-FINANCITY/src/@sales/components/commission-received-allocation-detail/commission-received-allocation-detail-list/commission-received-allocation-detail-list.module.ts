import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionReceiveAllocationDetailListComponent } from './commission-received-allocation-detail-list.component';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { CommissionReceivedAllocationDetailDetailModule } from '../commission-received-allocation-detail-detail/commission-received-allocation-detail-detail.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';

@NgModule({
  declarations: [CommissionReceiveAllocationDetailListComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    FormsModule,
    PWindowModule,
    PGridViewModule,
    CommissionReceivedAllocationDetailDetailModule,
    TextBoxModule,
  ],
  exports: [CommissionReceiveAllocationDetailListComponent]
})
export class CommissionReceiveAllocationListModule { }
