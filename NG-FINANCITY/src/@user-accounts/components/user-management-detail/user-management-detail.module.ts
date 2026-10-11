import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule,TextBoxModule } from '@progress/kendo-angular-inputs';
import { UserManagementDetailComponent } from './user-management-detail.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  entryComponents: [UserManagementDetailComponent],
  declarations: [UserManagementDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,TextBoxModule,
    DropDownsModule,
    PInputIconDirectiveModule
  ],
  exports: [UserManagementDetailComponent]
})
export class UserManagementDetailModule { }
