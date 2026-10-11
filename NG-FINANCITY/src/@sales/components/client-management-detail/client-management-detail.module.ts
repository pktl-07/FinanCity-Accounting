import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ClientManagementDetailComponent } from './client-management-detail.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ContentInformationModule } from '../@shared/content-information/content-information.module';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { ClientManagementTransactionHistoryModule } from './client-management-detail-transaction-history/client-management-detail-transaction-history.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { PImageUploaderModule } from 'src/@common/components/p-image-uploader/p-image-uploader.module';

@NgModule({
  entryComponents: [ClientManagementDetailComponent],
  declarations: [ClientManagementDetailComponent],
  imports: [
    CommonModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    LayoutModule,
    DropDownsModule,
    ContentInformationModule,
    InputsModule,
    PCustomKendoDatePickerModule,
    TextBoxModule,
    ClientManagementTransactionHistoryModule,
    PInputIconDirectiveModule,
    PImageUploaderModule
  ],
  exports: [ClientManagementDetailComponent]
})
export class ClientManagementDetailModule { }
