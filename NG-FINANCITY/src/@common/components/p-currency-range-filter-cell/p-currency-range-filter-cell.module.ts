import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GridModule } from '@progress/kendo-angular-grid';
import { CurrencyRangeFilterCellComponent } from './p-currency-range-filter-cell.component';
import { FormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';

@NgModule({
    declarations: [CurrencyRangeFilterCellComponent],
    imports: [
        GridModule,
        FormsModule,
        CommonModule,
        InputsModule,
    ],
    exports: [CurrencyRangeFilterCellComponent]
})
export class CurrencyRangeFilterCellModule { }
