import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { CommissionReceivedAllocationDetailComponent } from './commission-received-allocation-detail.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { CommissionReceiveAllocationListModule } from './commission-received-allocation-detail-list/commission-received-allocation-detail-list.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { CommissionReceivedDetailInvoiceModule } from '../commission-received-detail/commission-received-detail-invoice/commission-received-detail-invoice.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { PageUtilityService } from '../commission-received-detail/page.utility.service';
import { RouterModule } from '@angular/router';

@NgModule({
  entryComponents: [CommissionReceivedAllocationDetailComponent],
  declarations: [CommissionReceivedAllocationDetailComponent],
  imports: [
    CommonModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    LayoutModule,
    PToolbarModule,
    CommissionReceiveAllocationListModule,
    CommissionReceivedDetailInvoiceModule,
    TextBoxModule,
    RouterModule.forChild([{ path: '', component: CommissionReceivedAllocationDetailComponent }])
  ],
  exports: [CommissionReceivedAllocationDetailComponent],
  providers: [
    PageUtilityService
  ]

})
export class CommissoinReceivedAllocationDetailModule { }
