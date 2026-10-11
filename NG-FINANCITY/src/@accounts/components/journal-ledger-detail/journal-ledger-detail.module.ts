import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JournalLedgerDetailComponent } from './journal-ledger-detail.component';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PRecordStatusModule } from 'src/@common/components/p-recordstatus/p-recordstatus.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { MultiCurrencyModule } from 'src/@sales/components/@shared/multi-currency-dropdown/multi-currency-module';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { AccountSetupDetailModule } from '../account-setup-detail/account-setup-detail.module';
import { CurrencyRateDetailModule } from 'src/@sales/components/currency-rate-detail/currency-rate-detail.module';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { RecapDailogModule } from 'src/@common/components/p-recap-dialog/p-recap-dialog.module';
import { JobDetailModule } from '../job-detail/job-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [JournalLedgerDetailComponent],
  imports: [
    CommonModule,
    InputsModule,
    TextBoxModule,
    ButtonModule,
    FormsModule,
    ReactiveFormsModule,
    PToolbarModule,
    PRecordStatusModule,
    PInputIconDirectiveModule,
    PCustomKendoDatePickerModule,
    MultiCurrencyModule,
    PGridIncellEditingModule,
    AccountSetupDetailModule,
    CurrencyRateDetailModule,
    DropDownsModule,
    RecapDailogModule,
    JobDetailModule,
    RouterModule.forChild([{ path: '', component: JournalLedgerDetailComponent }])
  ]
})
export class JournalLedgerDetailModule { }
