import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { PDateFormatPipeModule } from '../../pipe/p-date-format-pipe/p-date-format-pipe-module';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { PopupModule } from '@progress/kendo-angular-popup';
import { FilterComponent } from '../p-filter-panel/p-filter-panel-component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PCustomKendoDatePickerModule } from '../p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { RippleModule } from '@progress/kendo-angular-ripple';
import { PDatePeriodModule } from '../p-date-period/p-date-period.module';

@NgModule({
    entryComponents: [FilterComponent],
    declarations: [FilterComponent],
    imports: [
        CommonModule,
        ButtonsModule,
        ReactiveFormsModule,
        FormsModule,
        PDateFormatPipeModule,
        TextBoxModule,
        TooltipModule,
        PopupModule,
        PCustomKendoDatePickerModule,
        InputsModule,
        DropDownsModule,
        RippleModule,
        PDatePeriodModule,
    ],
    exports: [FilterComponent]
})
export class FilterPanelModule { }
