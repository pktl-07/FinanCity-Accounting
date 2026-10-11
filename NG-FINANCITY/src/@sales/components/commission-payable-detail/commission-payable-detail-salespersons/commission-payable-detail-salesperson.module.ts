import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionPayableDetailSalespersonComponent } from './commission-payable-detail-salesperson.component';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
    declarations: [CommissionPayableDetailSalespersonComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        TextBoxModule,
        PInputIconDirectiveModule
    ],
    exports: [CommissionPayableDetailSalespersonComponent]
})
export class CommissionPayableDetailSalespersonlModule { }
