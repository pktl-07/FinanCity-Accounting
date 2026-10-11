import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountSetupDetailComponent } from './account-setup-detail.component';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  entryComponents: [AccountSetupDetailComponent],
  declarations: [AccountSetupDetailComponent],
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
    PInputIconDirectiveModule
  ],
  exports: [AccountSetupDetailComponent]
})
export class AccountSetupDetailModule { }
