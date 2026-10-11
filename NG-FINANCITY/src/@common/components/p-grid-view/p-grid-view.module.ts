import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GridModule } from '@progress/kendo-angular-grid';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateRangeFilterCellModule } from '../p-date-range-filter-cell/p-date-range-filter-cell.module';
import { PDateFormatPipeModule } from '../../pipe/p-date-format-pipe/p-date-format-pipe-module';
import { StringFilterCellModule } from '../p-string-input-filter-cell/p-string-input-filter-cell.module';
import { PGridViewComponent } from './p-grid-view.component';
import { CurrencyRangeFilterCellModule } from '../p-currency-range-filter-cell/p-currency-range-filter-cell.module';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { PopupModule } from '@progress/kendo-angular-popup';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PCustomKendoDatePickerModule } from '../p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PWindowModule } from '../p-window/p-window.module';
import { RippleModule } from '@progress/kendo-angular-ripple';
import { PDatePeriodModule } from '../p-date-period/p-date-period.module';
import { FilterPanelModule } from '../p-filter-panel/p-filter-panel-module';

@NgModule({

  declarations: [PGridViewComponent],
  imports: [
    TextBoxModule,
    CommonModule,
    GridModule,
    ButtonsModule,
    ReactiveFormsModule,
    FormsModule,
    PDateFormatPipeModule,
    StringFilterCellModule,
    DateRangeFilterCellModule,
    CurrencyRangeFilterCellModule,
    TooltipModule,
    PopupModule,
    PCustomKendoDatePickerModule,
    InputsModule,
    DropDownsModule,
    PWindowModule,
    RippleModule,
    PDatePeriodModule,
    FilterPanelModule
  ],
  exports: [PGridViewComponent]
})
export class PGridViewModule { }
