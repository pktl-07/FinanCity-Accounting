import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PropertyTransactionClientsComponent } from './property-transaction-clients.component';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { ClientManagementDetailModule } from '../../client-management-detail/client-management-detail.module';

@NgModule({
  declarations: [PropertyTransactionClientsComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PWindowModule,
    ClientManagementDetailModule,
    PGridIncellEditingModule,
  ],
  exports: [PropertyTransactionClientsComponent]
})
export class PropertyTransactionClientsModule { }
