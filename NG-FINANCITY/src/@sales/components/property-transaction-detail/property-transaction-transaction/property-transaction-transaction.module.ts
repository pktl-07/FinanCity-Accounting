import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PropertyTransactionTransactionComponent } from './property-transaction-transaction.component';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { MultiCurrencyModule } from '../../@shared/multi-currency-dropdown/multi-currency-module';
import { ContentInformationModule } from '../../@shared/content-information/content-information.module';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [PropertyTransactionTransactionComponent],
  imports: [
    CommonModule,
    ButtonModule,
    ReactiveFormsModule,
    DatePickerModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    DropDownsModule,
    PCustomKendoDatePickerModule,
    MultiCurrencyModule,
    ContentInformationModule,
    PInputIconDirectiveModule
  ],
  exports: [PropertyTransactionTransactionComponent]
})
export class PropertyTransactionTransactionModule { }
