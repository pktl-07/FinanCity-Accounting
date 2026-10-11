import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { AccountingPeriodCreateComponent } from './accounting-period-create.component';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';

@NgModule({
  entryComponents: [AccountingPeriodCreateComponent],
  declarations: [AccountingPeriodCreateComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    ButtonModule,
    PCustomKendoDatePickerModule,
    InputsModule,
    TextBoxModule,
    PInputIconDirectiveModule,
    
  ],
  exports: [AccountingPeriodCreateComponent]
})
export class AccountingPeriodCreateModule { }
