import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PromotionItemExpenseComponent } from './promotion-item-expense.component';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PromotionItemDetailModule } from '../../promotion-item-detail/promotion-item-detail.module';

@NgModule({
  declarations: [PromotionItemExpenseComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PWindowModule,
    PromotionItemDetailModule,
    PGridIncellEditingModule,
  ],
  exports: [PromotionItemExpenseComponent]
})
export class PromotionItemExpenseModule { }
