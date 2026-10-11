import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule,TextBoxModule } from '@progress/kendo-angular-inputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { VendorManagementDetailComponent } from './vendor-management-detail.component';
import { ContentInformationModule } from '../@shared/content-information/content-information.module';
import { TaxDetailModule } from 'src/@accounts/components/tax-detail/tax-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { PImageUploaderModule } from 'src/@common/components/p-image-uploader/p-image-uploader.module';

@NgModule({
  entryComponents: [VendorManagementDetailComponent],
  declarations: [VendorManagementDetailComponent],
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
    PInputIconDirectiveModule,
    PImageUploaderModule
  ],
  exports: [VendorManagementDetailComponent]
})
export class VendorManagementDetailModule { }
