import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PropertyTransactionExternalComponent } from './property-transaction-externals.component';
import { ExternalSalespersonDetailModule } from '../../external-salesperson-detail/external-salesperson-detail.module';

@NgModule({
  declarations: [PropertyTransactionExternalComponent],
  imports: [
    CommonModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    PGridIncellEditingModule,
    PWindowModule,
    ExternalSalespersonDetailModule
  ],
  exports: [PropertyTransactionExternalComponent]
})
export class PropertyTransactionExternalModule { }
