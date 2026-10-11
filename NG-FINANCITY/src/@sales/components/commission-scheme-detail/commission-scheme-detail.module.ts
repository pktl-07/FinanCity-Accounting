import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommissionSchemeDetailComponent } from './commission-scheme-detail.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  entryComponents: [CommissionSchemeDetailComponent],
  declarations: [CommissionSchemeDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    PGridIncellEditingModule,
    PInputIconDirectiveModule
  ],
  exports: [CommissionSchemeDetailComponent]
})
export class CommissionSchemeDetailModule { }
