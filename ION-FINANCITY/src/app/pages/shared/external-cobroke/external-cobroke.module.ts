import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ExternalCobrokePage } from './external-cobroke.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { CompanyListModalPageModule } from '../company-list-modal/company-list-modal.module';
import { FCModalSelectModule } from 'src/@common/components/modal-select/modal-select.module';

@NgModule({
  entryComponents: [ExternalCobrokePage],
  declarations: [ExternalCobrokePage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCValidatorModule,
    CompanyListModalPageModule,
    FCModalSelectModule,
  ],
  exports: [ExternalCobrokePage]
})
export class ExternalCobrokeModule { }
