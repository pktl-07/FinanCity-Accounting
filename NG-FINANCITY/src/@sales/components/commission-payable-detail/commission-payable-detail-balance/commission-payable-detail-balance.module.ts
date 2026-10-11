import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionPayableDetailBalanceComponent } from './commission-payable-detail-balance.component';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
    declarations: [CommissionPayableDetailBalanceComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        InputsModule,
        TextBoxModule,
        PInputIconDirectiveModule
    ],
    exports: [CommissionPayableDetailBalanceComponent]
})
export class CommissionPayableDetailBalanceModule { }
