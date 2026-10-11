import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionPayableDetailPaymentComponent } from './commission-payable-detail-payment.component';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { CommissionPayableDetailPaidModule } from '../commission-payable-detail-paid/commission-payable-detail-paid.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';

@NgModule({
    declarations: [CommissionPayableDetailPaymentComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        InputsModule,
        PGridViewModule,
        CommissionPayableDetailPaidModule,
        PWindowModule,
        TextBoxModule,
    ],
    exports: [CommissionPayableDetailPaymentComponent]
})
export class CommissionPayableDetailPaymentModule { }
