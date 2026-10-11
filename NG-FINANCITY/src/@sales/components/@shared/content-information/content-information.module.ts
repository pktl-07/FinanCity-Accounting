import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { ContentInformationComponent } from './content-information.component';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [ContentInformationComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    PInputIconDirectiveModule
  ],
  exports: [ContentInformationComponent]
})
export class ContentInformationModule { }
