import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionReceivedDetailInvoiceComponent } from './commission-received-detail-invoice.component';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [CommissionReceivedDetailInvoiceComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    PCustomKendoDatePickerModule,
    TextBoxModule,
    PInputIconDirectiveModule
  ],
  exports: [CommissionReceivedDetailInvoiceComponent],
})
export class CommissionReceivedDetailInvoiceModule { }
