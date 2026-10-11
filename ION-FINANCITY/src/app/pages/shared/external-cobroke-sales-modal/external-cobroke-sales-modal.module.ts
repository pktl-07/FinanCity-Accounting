import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ExternalSalesSalesModalPage } from './external-cobroke-sales-modal.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { ExternalCobrokeListModalModule } from '../external-cobroke-list-modal/external-cobroke-list-modal.module';
import { FCModalSelectModule } from 'src/@common/components/modal-select/modal-select.module';
import { FCCurrencyFormatModule } from 'src/@common/directives/fc-directive.module';

@NgModule({
  entryComponents: [ExternalSalesSalesModalPage],
  declarations: [ExternalSalesSalesModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCValidatorModule,
    ExternalCobrokeListModalModule,
    FCModalSelectModule,
    FCCurrencyFormatModule
  ],
  exports: [ExternalSalesSalesModalPage]
})
export class ExternalSalespersonSalesModalModule { }
