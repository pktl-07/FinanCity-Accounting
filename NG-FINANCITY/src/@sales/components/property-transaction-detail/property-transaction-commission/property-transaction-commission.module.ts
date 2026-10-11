import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PropertyTransactionCommissionComponent } from './property-transaction-commission.component';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { TaxDetailModule } from 'src/@accounts/components/tax-detail/tax-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PCommissionStructurePipeModule } from 'src/@common/pipe/p-commission-structure-pipe/p-commission-structure-pipe.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [PropertyTransactionCommissionComponent],
  imports: [
    CommonModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    DropDownsModule,
    TaxDetailModule,
    PWindowModule,
    PCommissionStructurePipeModule,
    PInputIconDirectiveModule
  ],
  exports: [PropertyTransactionCommissionComponent]
})
export class PropertyTransactionCommissionModule { }
