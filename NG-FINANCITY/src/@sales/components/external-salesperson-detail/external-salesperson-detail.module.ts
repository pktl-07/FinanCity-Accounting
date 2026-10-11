import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ExternalSalespersonDetailComponent } from './external-salesperson-detail.component';
import { ContentInformationModule } from '../@shared/content-information/content-information.module';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { InputsModule,TextBoxModule } from '@progress/kendo-angular-inputs';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { VendorManagementDetailModule } from '../vendor-management-detail/vendor-management-detail.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  entryComponents: [ExternalSalespersonDetailComponent],
  declarations: [ExternalSalespersonDetailComponent],
  imports: [
    CommonModule,
    PModalFormModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,TextBoxModule,
    PCustomKendoDatePickerModule,
    PWindowModule,
    VendorManagementDetailModule,
    ContentInformationModule,
    LayoutModule,
    PToolbarModule,
    PInputIconDirectiveModule
  ],
  exports: [ExternalSalespersonDetailComponent]
})
export class ExternalSalespersonDetailModule { }
