import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CashbookSetupDetailComponent } from './cashbook-setup-detail.component';
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

@NgModule({
  entryComponents: [CashbookSetupDetailComponent],
  declarations: [CashbookSetupDetailComponent],
  imports: [
    CommonModule,
    InputsModule,
    TextBoxModule,
    ButtonModule,
    DropDownsModule,
    FormsModule,
    ReactiveFormsModule,
    PModalFormModule,
    PToolbarModule,
    PWindowModule,
    CurrencyRateDetailModule,
    AccountSetupDetailModule,
    PInputIconDirectiveModule
  ],
  exports: [CashbookSetupDetailComponent]
})
export class CashbookSetupDetailModule { }
