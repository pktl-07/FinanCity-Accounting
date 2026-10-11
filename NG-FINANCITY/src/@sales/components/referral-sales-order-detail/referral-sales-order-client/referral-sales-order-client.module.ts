import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { ClientManagementDetailModule } from '../../client-management-detail/client-management-detail.module';
import { ReferralSalesOrderClientsComponent } from './referral-sales-order-client.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [ReferralSalesOrderClientsComponent],
  imports: [
    CommonModule,
    DropDownsModule,
    ButtonModule,
    ReactiveFormsModule,
    FormsModule,
    PWindowModule,
    ClientManagementDetailModule,
    PGridIncellEditingModule,
    PInputIconDirectiveModule
  ],
  exports: [ReferralSalesOrderClientsComponent]
})
export class ReferralSalesOrderClientModule { }
