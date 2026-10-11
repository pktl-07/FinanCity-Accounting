import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SalespersonManagementPayNowComponent } from "./salesperson-paynow.component";
import { TextBoxModule } from '@progress/kendo-angular-inputs';

@NgModule({
  declarations: [SalespersonManagementPayNowComponent],
  imports: [
    CommonModule,
    TextBoxModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  exports: [SalespersonManagementPayNowComponent]
})
export class SalespersonManagementPayNowModule { }
