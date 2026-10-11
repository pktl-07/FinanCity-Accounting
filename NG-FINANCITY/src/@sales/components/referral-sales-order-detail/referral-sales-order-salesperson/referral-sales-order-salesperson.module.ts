import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { SalesOrderTypeDetailModule } from '../../sales-order-type-detail/sales-order-type-detail.module';
import { SalespersonManagementDetailModule } from '../../salesperson-management-detail/salesperson-management-detail.module';
import { ReferralSalesOrderSalespersonComponent } from './referral-sales-order-salesperson.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [ReferralSalesOrderSalespersonComponent],
  imports: [
    CommonModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    PWindowModule,
    DropDownsModule,
    InputsModule,
    SalesOrderTypeDetailModule,
    PGridIncellEditingModule,
    SalespersonManagementDetailModule,
    PInputIconDirectiveModule
  ],
  exports: [ReferralSalesOrderSalespersonComponent]
})
export class ReferralSalesOrderSalespersonModule { }
