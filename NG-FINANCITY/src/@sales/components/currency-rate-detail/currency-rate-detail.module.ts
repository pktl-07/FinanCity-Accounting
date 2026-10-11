import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { InputsModule,TextBoxModule } from '@progress/kendo-angular-inputs';
import { CurrencyRateDetailComponent } from './currency-rate-detail.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PGridIncellEditingModule } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  entryComponents: [CurrencyRateDetailComponent],
  declarations: [CurrencyRateDetailComponent],
  imports: [
    CommonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,TextBoxModule,
    PModalFormModule,
    PToolbarModule,
    PGridIncellEditingModule,
    PInputIconDirectiveModule
  ],
  exports: [CurrencyRateDetailComponent]
})
export class CurrencyRateDetailModule { }
