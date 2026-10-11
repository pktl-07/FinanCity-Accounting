import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PropertyTransactionInternalComponent } from './property-transaction-internals.component';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { SalespersonManagementDetailModule } from '../../salesperson-management-detail/salesperson-management-detail.module';

@NgModule({
  declarations: [PropertyTransactionInternalComponent],
  imports: [
    CommonModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    PGridIncellEditingModule,
    PWindowModule,
    SalespersonManagementDetailModule
  ],
  exports: [PropertyTransactionInternalComponent]
})
export class PropertyTransactionInternalModule { }
