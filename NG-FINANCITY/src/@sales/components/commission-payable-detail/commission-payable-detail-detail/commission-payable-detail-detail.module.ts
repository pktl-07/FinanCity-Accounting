import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommissionPayableDetailDetailComponent } from './commission-payable-detail-detail.component';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PCommissionStructurePipeModule } from 'src/@common/pipe/p-commission-structure-pipe/p-commission-structure-pipe.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
    declarations: [CommissionPayableDetailDetailComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        InputsModule,
        TextBoxModule,
        PCommissionStructurePipeModule,
        PInputIconDirectiveModule
    ],
    exports: [CommissionPayableDetailDetailComponent]
})
export class CommissionPayableDetailDetailModule { }
