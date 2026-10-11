import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ReferralSalesOrderDetailComponent } from './referral-sales-order-detail.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { PRecordStatusModule } from 'src/@common/components/p-recordstatus/p-recordstatus.module';
import { PropertyTransactionActivityModule } from '../property-transaction-detail/property-transaction-activity/property-transaction-activity.module';
import { PropertyTransactionTransactionModule } from '../property-transaction-detail/property-transaction-transaction/property-transaction-transaction.module';
import { PropertyTransactionBillToCobrokeModule } from '../property-transaction-detail/property-transaction-billtocobroke/property-transaction-billtocobroke.module';
import { ReferralSalesOrderClientModule } from './referral-sales-order-client/referral-sales-order-client.module';
import { ReferralSalesOrderSalespersonModule } from './referral-sales-order-salesperson/referral-sales-order-salesperson.module';
import { PageUtilityService } from '../property-transaction-detail/page.utility.service';
import { ReferralSalesOrderCommissionModule } from './refferral-sales-order-commission/refferral-sales-order-commission.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ReferralSalesOrderDetailComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    DatePickerModule,
    FormsModule,
    InputsModule,
    LayoutModule,
    PToolbarModule,
    PRecordStatusModule,
    PropertyTransactionActivityModule,
    PropertyTransactionTransactionModule,
    PropertyTransactionBillToCobrokeModule,
    ReferralSalesOrderClientModule,
    ReferralSalesOrderSalespersonModule,
    ReferralSalesOrderCommissionModule,
    RouterModule.forChild([{ path: '', component: ReferralSalesOrderDetailComponent }])
  ],
  providers: [PageUtilityService]
})
export class ReferralSalesOrderDetailModule { }
