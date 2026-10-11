import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SalespersonManagementBankComponent } from "./salesperson-management-bank.component";
import { TextBoxModule } from '@progress/kendo-angular-inputs';

@NgModule({
  declarations: [SalespersonManagementBankComponent],
  imports: [
    CommonModule,
    TextBoxModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  exports: [SalespersonManagementBankComponent]
})
export class SalespersonManagementBankModule { }
