import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { SalespersonSalesModalPage } from './salesperson-sales-modal.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { SalespersonListModalModule } from '../salesperson-list-modal/salesperson-list-modal.module';
import { FCModalSelectModule } from 'src/@common/components/modal-select/modal-select.module';
import { FCCurrencyFormatModule } from 'src/@common/directives/fc-directive.module';

@NgModule({
  entryComponents: [SalespersonSalesModalPage],
  declarations: [SalespersonSalesModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCValidatorModule,
    FCCurrencyFormatModule,
    SalespersonListModalModule,
    FCModalSelectModule
  ],
  exports: [SalespersonSalesModalPage]
})
export class SalespersonSalesModalModule { }
