import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PDatePeriodComponent } from './p-date-period.component';
import { PCustomKendoDatePickerModule } from '../p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';

@NgModule({
  declarations: [
    PDatePeriodComponent
  ],
  imports: [
    CommonModule,
    DatePickerModule,
    ReactiveFormsModule,
    FormsModule,
    PCustomKendoDatePickerModule,
    DropDownsModule
  ],
  exports: [
    PDatePeriodComponent,
  ],
})
export class PDatePeriodModule { }
