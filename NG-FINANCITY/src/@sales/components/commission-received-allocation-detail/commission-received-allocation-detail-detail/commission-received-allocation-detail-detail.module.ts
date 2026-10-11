import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionReceivedAllocationDetailDetailComponent } from './commission-received-allocation-detail-detail.component';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { DropDownListModule } from '@progress/kendo-angular-dropdowns';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PRecordStatusModule } from 'src/@common/components/p-recordstatus/p-recordstatus.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { CashbookSetupDetailModule } from 'src/@accounts/components/cashbook-setup-detail/cashbook-setup-detail.module';
import { RecapDailogModule } from 'src/@common/components/p-recap-dialog/p-recap-dialog.module';

@NgModule({
  entryComponents: [CommissionReceivedAllocationDetailDetailComponent],
  declarations: [CommissionReceivedAllocationDetailDetailComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    FormsModule,
    InputsModule,
    PModalFormModule,
    PWindowModule,
    PToolbarModule,
    PCustomKendoDatePickerModule,
    CashbookSetupDetailModule,
    DropDownListModule,
    TextBoxModule,
    PRecordStatusModule,
    PInputIconDirectiveModule,
    RecapDailogModule
  ],
  exports: [CommissionReceivedAllocationDetailDetailComponent]
})
export class CommissionReceivedAllocationDetailDetailModule { }
