import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PropertyTransactionActivityComponent } from './property-transaction-activity.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PSalesStatusModule } from 'src/@common/components/p-sales-status/p-sales-status.module';

@NgModule({
  declarations: [PropertyTransactionActivityComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PGridViewModule,
    PSalesStatusModule
  ],
  exports: [PropertyTransactionActivityComponent]
})
export class PropertyTransactionActivityModule { }
