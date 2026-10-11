import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { SalespersonManagementDetailComponent } from './salesperson-management-detail.component'
import { LayoutModule, TabStripModule } from '@progress/kendo-angular-layout';
import { ContentInformationModule } from '../@shared/content-information/content-information.module';
import { SalespersonCommissionSchemeModule } from './salesperson-commission-scheme/salesperson-commission-schememodule';
import { SalespersonTierModule } from './salesperson-tier/salesperson-tier.module';
import { SalespersonEmployemntModule } from './salesperson-employment/salesperson-employment.module';
import { SalespersonProfileModule } from './salesperson-profile/salesperson-profile.module';
import { SalespersonCareerActivityModule } from './salesperson-career-activity/salesperson-career-activity.module';
import { SalespersonManagementBankModule } from './salesperson-management-bank/salesperson-management-bank.module';
import { SalespersonManagementPayNowModule } from './salesperson-paynow/salesperson-paynow.module';

@NgModule({
  entryComponents: [SalespersonManagementDetailComponent],
  declarations: [SalespersonManagementDetailComponent],
  imports: [
    CommonModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    LayoutModule,
    ContentInformationModule,
    SalespersonCommissionSchemeModule,
    SalespersonTierModule,
    SalespersonEmployemntModule,
    SalespersonProfileModule,
    SalespersonManagementBankModule,
    SalespersonCareerActivityModule,
    PModalFormModule,
    SalespersonManagementPayNowModule,

  ],
  exports: [SalespersonManagementDetailComponent]
})
export class SalespersonManagementDetailModule { }
