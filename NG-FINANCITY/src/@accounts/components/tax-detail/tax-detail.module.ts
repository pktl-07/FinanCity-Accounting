import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaxDetailComponent } from './tax-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { AccountSetupDetailModule } from '../account-setup-detail/account-setup-detail.module';

@NgModule({
  entryComponents: [TaxDetailComponent],
  declarations: [TaxDetailComponent],
  imports: [
    CommonModule,
    PModalFormModule,
    ButtonModule,
    FormsModule,
    ReactiveFormsModule,
    DropDownsModule,
    InputsModule,
    PWindowModule,
    AccountSetupDetailModule,
    TextBoxModule,
  ],
  exports: [TaxDetailComponent]
})
export class TaxDetailModule {
}
