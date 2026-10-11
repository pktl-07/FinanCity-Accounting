import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { FinancialCalculatorPage } from './financial-calculator.page';
import { FCCurrencyFormatModule, FCDecimalFormatModule } from 'src/@common/directives/fc-directive.module';

@NgModule({
  declarations: [FinancialCalculatorPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCDecimalFormatModule,
    FCCurrencyFormatModule,
    FCValidatorModule,
  ],
  exports: [FinancialCalculatorPage]
})
export class FinantialCalculatorPageModule { }
