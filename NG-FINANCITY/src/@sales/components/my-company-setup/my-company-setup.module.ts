import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { MyCompanySetupComponent } from './my-company-setup.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { TaxDetailModule } from 'src/@accounts/components/tax-detail/tax-detail.module';
import { PImageUploaderModule } from 'src/@common/components/p-image-uploader/p-image-uploader.module';
import { ContentInformationModule } from '../@shared/content-information/content-information.module';
import { RouterModule } from '@angular/router';

@NgModule({

  declarations: [MyCompanySetupComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule, TextBoxModule,
    LayoutModule,
    PToolbarModule,
    PCustomKendoDatePickerModule,
    PWindowModule,
    TaxDetailModule,
    PImageUploaderModule,
    ContentInformationModule,
    RouterModule.forChild([{ path: '', component: MyCompanySetupComponent }])
  ]
})
export class MyCompanySetupModule { }
