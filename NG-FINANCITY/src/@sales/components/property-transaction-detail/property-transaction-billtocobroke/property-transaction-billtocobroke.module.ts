import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PropertyTransactionBillToCobrokeComponent } from './property-transaction-billtocobroke.component';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { VendorManagementDetailModule } from '../../vendor-management-detail/vendor-management-detail.module';
import { PWindowModule } from '../../../../@common/components/p-window/p-window.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { ExternalSalespersonDetailModule } from '../../external-salesperson-detail/external-salesperson-detail.module';

@NgModule({
  declarations: [PropertyTransactionBillToCobrokeComponent],
  imports: [
    CommonModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule, TextBoxModule,
    DropDownsModule,
    VendorManagementDetailModule,
    ExternalSalespersonDetailModule,
    PWindowModule,
    PInputIconDirectiveModule
  ],
  exports: [PropertyTransactionBillToCobrokeComponent]
})
export class PropertyTransactionBillToCobrokeModule { }
