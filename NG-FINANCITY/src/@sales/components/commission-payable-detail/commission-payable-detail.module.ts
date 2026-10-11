import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionPayableDetailComponent } from './commission-payable-detail.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { CommissionPayableDetailSalespersonlModule } from './commission-payable-detail-salespersons/commission-payable-detail-salesperson.module';
import { CommissionPayableDetailDetailModule } from './commission-payable-detail-detail/commission-payable-detail-detail.module';
import { CommissionPayableDetailSchemeModule } from './commission-payable-detail-scheme/commission-payable-detail-scheme.module';
import { CommissionPayableDetailBalanceModule } from './commission-payable-detail-balance/commission-payable-detail-balance.module';
import { CommissionPayableDetailPaymentModule } from './commission-payable-detail-payment/commission-payable-detail-payment.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { PageUtilityService } from './page.utility.service';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CommissionPayableDetailComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PToolbarModule,
    FormsModule,
    LayoutModule,
    CommissionPayableDetailSalespersonlModule,
    CommissionPayableDetailDetailModule,
    CommissionPayableDetailSchemeModule,
    CommissionPayableDetailBalanceModule,
    CommissionPayableDetailPaymentModule,
    TextBoxModule,
    RouterModule.forChild([{ path: '', component: CommissionPayableDetailComponent }])
  ],
  exports: [CommissionPayableDetailComponent],
  providers: [PageUtilityService]
})
export class CommissionPayableDetailModule { }
