import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionReceiveReceivableListComponent } from './commission-received-receivable-list.component';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { CommissionReceivedDetailDetailModule } from '../commission-received-detail-detail/commission-received-detail-detail.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';

@NgModule({
  declarations: [CommissionReceiveReceivableListComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    FormsModule,
    PWindowModule,
    PGridViewModule,
    CommissionReceivedDetailDetailModule,
    TextBoxModule,
  ],
  exports: [CommissionReceiveReceivableListComponent]
})
export class CommissionReceiveReceivableListModule { }
