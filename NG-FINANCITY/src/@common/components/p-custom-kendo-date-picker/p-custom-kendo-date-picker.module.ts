import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { PCustomKendoDatePickerComponent } from './p-custom-kendo-date-picker.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { IntlModule } from '@progress/kendo-angular-intl';

@NgModule({
  declarations: [PCustomKendoDatePickerComponent],
  imports: [
    CommonModule,
    DatePickerModule,
    ReactiveFormsModule,
    FormsModule,
    IntlModule,
  ],
  exports: [PCustomKendoDatePickerComponent],
})
export class PCustomKendoDatePickerModule { }
