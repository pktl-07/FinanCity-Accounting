import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { SalesOrderTypeDetailComponent } from './sales-order-type-detail.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { AccountSetupDetailModule } from 'src/@accounts/components/account-setup-detail/account-setup-detail.module';

@NgModule({
  entryComponents: [SalesOrderTypeDetailComponent],
  declarations: [SalesOrderTypeDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    AccountSetupDetailModule,
  ],
  exports: [SalesOrderTypeDetailComponent]
})
export class SalesOrderTypeDetailModule { }
