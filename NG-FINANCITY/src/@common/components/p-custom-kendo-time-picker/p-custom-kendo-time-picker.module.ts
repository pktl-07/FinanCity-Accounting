import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimePickerModule } from '@progress/kendo-angular-dateinputs';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PCustomKendoTimePickerComponent } from './p-custom-kendo-time-picker.component';

@NgModule({
  declarations: [PCustomKendoTimePickerComponent],
  imports: [
    CommonModule,
    TimePickerModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  exports: [PCustomKendoTimePickerComponent],
})
export class PCustomKendoTimePickerModule { }
