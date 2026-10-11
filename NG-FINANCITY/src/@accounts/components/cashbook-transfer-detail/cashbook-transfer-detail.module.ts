import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { CurrencyRateDetailModule } from 'src/@sales/components/currency-rate-detail/currency-rate-detail.module';
import { AccountSetupDetailModule } from '../account-setup-detail/account-setup-detail.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { CashbookTransferDetailComponent } from './cashbook-transfer-detail.component';
import { PRecordStatusModule } from 'src/@common/components/p-recordstatus/p-recordstatus.module';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { RecapDailogModule } from 'src/@common/components/p-recap-dialog/p-recap-dialog.module';

@NgModule({
  entryComponents: [CashbookTransferDetailComponent],
  declarations: [CashbookTransferDetailComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputsModule,
    TextBoxModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    PToolbarModule,
    PWindowModule,
    CurrencyRateDetailModule,
    AccountSetupDetailModule,
    PInputIconDirectiveModule,
    PRecordStatusModule,
    PCustomKendoDatePickerModule,
    RecapDailogModule
  ],
  exports: [CashbookTransferDetailComponent]
})
export class CashbookTransferDetailModule { }
