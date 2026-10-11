import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GridModule } from '@progress/kendo-angular-grid';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { DateRangeFilterCellComponent } from './p-date-range-filter-cell.component';
import { FormsModule } from '@angular/forms';
import { IntlModule } from '@progress/kendo-angular-intl';

@NgModule({
  declarations: [DateRangeFilterCellComponent],
  imports: [
    GridModule,
    FormsModule,
    CommonModule,
    DatePickerModule,
    IntlModule,
  ],
  exports: [DateRangeFilterCellComponent]
})
export class DateRangeFilterCellModule { }
