import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { PRecordStatusModule } from 'src/@common/components/p-recordstatus/p-recordstatus.module';
import { CashbookTransactionDetailComponent } from './cashbook-transaction-detail.component';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { AccountSetupDetailModule } from '../account-setup-detail/account-setup-detail.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { RecapDailogModule } from 'src/@common/components/p-recap-dialog/p-recap-dialog.module';
import { RouterModule } from '@angular/router';
import { PrintingTemplate } from './printing-template/printing-template';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';

@NgModule({
  declarations: [CashbookTransactionDetailComponent, PrintingTemplate],
  imports: [
    CommonModule,
    InputsModule,
    TextBoxModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    DatePickerModule,
    FormsModule,
    PToolbarModule,
    PRecordStatusModule,
    PInputIconDirectiveModule,
    PCustomKendoDatePickerModule,
    PGridIncellEditingModule,

    PModalFormModule,
    PWindowModule,
    FilterPanelModule,

    AccountSetupDetailModule,
    RecapDailogModule,
    RouterModule.forChild([{ path: '', component: CashbookTransactionDetailComponent }])
  ],
  exports: [CashbookTransactionDetailComponent, PrintingTemplate]
})
export class CashbookTransactionDetailModule { }
