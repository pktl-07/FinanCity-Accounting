import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { BankBestRateDetailComponent } from './bank-best-rate-detail.component';
import { PFileUploaderModule } from 'src/@common/components/p-file-uploader/p-file-uploader.module';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PImageUploaderModule } from 'src/@common/components/p-image-uploader/p-image-uploader.module';

@NgModule({
  entryComponents: [BankBestRateDetailComponent],
  declarations: [BankBestRateDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    PFileUploaderModule,
    PCustomKendoDatePickerModule,
    PImageUploaderModule,
  ],
  exports: [BankBestRateDetailComponent]
})
export class BankBestRateDetailModule { }
