import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { PropertyTransactionTransactionModule } from './property-transaction-transaction/property-transaction-transaction.module';
import { PropertyTransactionClientsModule } from './property-transaction-clients/property-transaction-clients.module';
import { PropertyTransactionCommissionModule } from './property-transaction-commission/property-transaction-commission.module';
import { PropertyTransactionInternalModule } from './property-transaction-internals/property-transaction-internals.module';
import { PropertyTransactionExternalModule } from './property-transaction-externals/property-transaction-externals.module';
import { PropertyTransactionDetailComponent } from './property-transaction-detail.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PropertyTransactionTNCModule } from './property-transaction-tnc/property-transaction-tnc.module';
import { PropertyTransactionBillToCobrokeModule } from './property-transaction-billtocobroke/property-transaction-billtocobroke.module';
import { PageUtilityService } from './page.utility.service';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { PRecordStatusModule } from 'src/@common/components/p-recordstatus/p-recordstatus.module';
import { PropertyTransactionActivityModule } from './property-transaction-activity/property-transaction-activity.module';
import { RouterModule } from '@angular/router';
import { PromotionItemExpenseModule } from './promotion-item-expense/promotion-item-expense.module';

@NgModule({
  declarations: [PropertyTransactionDetailComponent],
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
    PropertyTransactionTransactionModule,
    PropertyTransactionClientsModule,
    PropertyTransactionCommissionModule,
    PropertyTransactionInternalModule,
    PropertyTransactionExternalModule,
    PropertyTransactionTNCModule,
    PropertyTransactionBillToCobrokeModule,
    PropertyTransactionActivityModule,
    PromotionItemExpenseModule,
    RouterModule.forChild([{ path: '', component: PropertyTransactionDetailComponent }])
  ],
  providers: [PageUtilityService]
})
export class PropertyTransactionDetailModule { }
