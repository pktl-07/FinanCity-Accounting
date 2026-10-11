import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PropertyTransactionTNCComponent } from './property-transaction-tnc.component';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PromotionItemExpenseModule } from '../promotion-item-expense/promotion-item-expense.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [PropertyTransactionTNCComponent],
  imports: [
    CommonModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    DropDownsModule,
    PromotionItemExpenseModule,
    PInputIconDirectiveModule
  ],
  exports: [PropertyTransactionTNCComponent]
})
export class PropertyTransactionTNCModule { }
