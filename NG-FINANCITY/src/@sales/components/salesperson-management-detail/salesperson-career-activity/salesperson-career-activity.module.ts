import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CareerActivityComponent } from './salesperson-career-activity.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';

@NgModule({
  declarations: [CareerActivityComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    PGridViewModule
  ],
  exports: [CareerActivityComponent]
})
export class SalespersonCareerActivityModule { }
