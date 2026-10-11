import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule,TextBoxModule } from '@progress/kendo-angular-inputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { FranchiseeManagementDetailComponent } from './franchisee-management-detail.component';
import { ContentInformationModule } from '../@shared/content-information/content-information.module';
import { TaxDetailModule } from 'src/@accounts/components/tax-detail/tax-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';

@NgModule({
  entryComponents: [FranchiseeManagementDetailComponent],
  declarations: [FranchiseeManagementDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    ReactiveFormsModule,
    PCustomKendoDatePickerModule,
    FormsModule,
    InputsModule,TextBoxModule,
    LayoutModule,
    ContentInformationModule,
    TaxDetailModule,
    PWindowModule,
  ],
  exports: [FranchiseeManagementDetailComponent]
})
export class FranchiseeManagementDetailModule { }
