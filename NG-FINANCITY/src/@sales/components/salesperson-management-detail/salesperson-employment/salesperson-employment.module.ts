import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { SalespersonEmploymentComponent } from './salesperson-employment.component';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { IndustryExamDetailModule } from '../../industryexam-detail/industryexam-detail.module';
import { QualificationDetailModule } from '../../qualification-detail/qualification-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [SalespersonEmploymentComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    PCustomKendoDatePickerModule,
    IndustryExamDetailModule,
    QualificationDetailModule,
    PWindowModule,
    PInputIconDirectiveModule
  ],
  exports: [SalespersonEmploymentComponent]
})
export class SalespersonEmployemntModule { }
