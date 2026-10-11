import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { SalespersonProfileComponent } from "./salesperson-profile.component";
import { DepartmentService } from 'src/@accounts/services/department.service';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PImageUploaderModule } from 'src/@common/components/p-image-uploader/p-image-uploader.module';
import { DepartmentDetailModule } from '../../department-detail/department-detail.module';
import { VendorService } from 'src/@sales/services/vendor.service';
import { FranchiseeManagementDetailModule } from '../../franchisee-management-detail/franchisee-management-detailmodule';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [SalespersonProfileComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule, TextBoxModule,
    PCustomKendoDatePickerModule,
    PImageUploaderModule,
    DepartmentDetailModule,
    FranchiseeManagementDetailModule,
    PGridViewModule,
    PInputIconDirectiveModule
  ],
  exports: [SalespersonProfileComponent]
})
export class SalespersonProfileModule { }
