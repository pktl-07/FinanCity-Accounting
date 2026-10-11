import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { InputsModule,TextBoxModule } from '@progress/kendo-angular-inputs';
import { DepartmentDetailComponent } from './department-detail.component';

@NgModule({
  entryComponents: [DepartmentDetailComponent],
  declarations: [DepartmentDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,TextBoxModule,
  ],
  exports: [DepartmentDetailComponent]
})
export class DepartmentDetailModule { }
