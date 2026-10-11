import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { MultiCurrencyComponent } from './multi-currency-component';
import { CurrencyRateDetailModule } from '../../currency-rate-detail/currency-rate-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [MultiCurrencyComponent],
  imports: [
    CommonModule,
    DropDownsModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    CurrencyRateDetailModule,
    PWindowModule,
    TextBoxModule,
    PInputIconDirectiveModule
  ],
  exports: [MultiCurrencyComponent]
})
export class MultiCurrencyModule { }
