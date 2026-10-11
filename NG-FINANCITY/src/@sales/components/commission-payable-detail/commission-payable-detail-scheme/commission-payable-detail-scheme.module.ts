import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { CommissionPayableDetailSchemeComponent } from './commission-payable-detail-scheme.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';

@NgModule({
    declarations: [CommissionPayableDetailSchemeComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        ButtonModule,
        FormsModule,
        InputsModule,
        PGridViewModule,
        TextBoxModule,
    ],
    exports: [CommissionPayableDetailSchemeComponent],
})
export class CommissionPayableDetailSchemeModule { }
