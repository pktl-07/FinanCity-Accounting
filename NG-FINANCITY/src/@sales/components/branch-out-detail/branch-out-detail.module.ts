import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { PRecordStatusModule } from 'src/@common/components/p-recordstatus/p-recordstatus.module';
import { BranchOutDetailComponent } from './branch-out-detail.component';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [BranchOutDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    DatePickerModule,
    FormsModule,
    InputsModule,
    PToolbarModule,
    PRecordStatusModule,
    PCustomKendoDatePickerModule,
    PInputIconDirectiveModule,
    TreeViewModule,
    RouterModule.forChild([{ path: '', component: BranchOutDetailComponent }])
  ]
})
export class BranchOutDetailModule { }
