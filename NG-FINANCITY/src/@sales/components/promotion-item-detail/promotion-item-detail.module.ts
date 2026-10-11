import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PromotionItemDetailComponent } from './promotion-item-detail.component';

@NgModule({
  entryComponents: [PromotionItemDetailComponent],
  declarations: [PromotionItemDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
  ],
  exports: [PromotionItemDetailComponent]
})
export class PromotionItemDetailModule { }
