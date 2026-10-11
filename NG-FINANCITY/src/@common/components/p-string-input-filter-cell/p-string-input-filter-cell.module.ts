import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GridModule } from '@progress/kendo-angular-grid';
import { StringFilterCellComponent } from './p-string-input-filter-cell.component';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';

@NgModule({
  declarations: [StringFilterCellComponent],
  imports: [
    GridModule,
    FormsModule,
    ButtonModule,
    CommonModule,
  ],
  exports: [StringFilterCellComponent]
})
export class StringFilterCellModule { }
