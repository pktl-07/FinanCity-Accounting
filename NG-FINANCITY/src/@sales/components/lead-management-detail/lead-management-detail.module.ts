import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ContentInformationModule } from '../@shared/content-information/content-information.module';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { LeadManagementDetailComponent } from './lead-management-detail.component';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';
import { LeadManagementDetailShareModule } from './lead-management-share/lead-management-detail-share.module';
import { LeadManagementRemarkModule } from './lead-management-remarks/lead-management-remarks.module';
import { PCustomKendoTimePickerModule } from 'src/@common/components/p-custom-kendo-time-picker/p-custom-kendo-time-picker.module';
import { PImageUploaderModule } from 'src/@common/components/p-image-uploader/p-image-uploader.module';

const components = [LeadManagementDetailComponent];

@NgModule({
  entryComponents: [components],
  declarations: [components],
  imports: [
    CommonModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    LayoutModule,
    ButtonModule,
    DropDownsModule,
    ContentInformationModule,
    InputsModule,
    PCustomKendoDatePickerModule,
    PCustomKendoTimePickerModule,
    TextBoxModule,
    PInputIconDirectiveModule,
    PConfirmDialogModule,
    LeadManagementDetailShareModule,
    LeadManagementRemarkModule,
    PImageUploaderModule,
  ],
  exports: [components]
})
export class LeadManagementDetailModule { }
