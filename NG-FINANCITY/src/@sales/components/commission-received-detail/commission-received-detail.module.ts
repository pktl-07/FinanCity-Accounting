import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { CommissionReceivedDetailComponent } from './commission-received-detail.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { CommissionReceivedDetailInvoiceModule } from './commission-received-detail-invoice/commission-received-detail-invoice.module';
import { CommissionReceiveReceivableListModule } from './commission-received-receivable-list/commission-received-receivable-list.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { PageUtilityService } from './page.utility.service';
import { RouterModule } from '@angular/router';

@NgModule({
  entryComponents: [],
  declarations: [CommissionReceivedDetailComponent],
  imports: [
    CommonModule,
    PModalFormModule,
    PToolbarModule,
    ReactiveFormsModule,
    FormsModule,
    LayoutModule,
    CommissionReceivedDetailInvoiceModule,
    CommissionReceiveReceivableListModule,
    TextBoxModule,
    RouterModule.forChild([{ path: '', component: CommissionReceivedDetailComponent }])
  ],
  exports: [CommissionReceivedDetailComponent],
  providers: [PageUtilityService],
})
export class CommissionReceivedDetailModule { }
