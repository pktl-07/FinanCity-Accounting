import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { LeadPage } from './lead.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { FCDateFormatModule, FCDecimalFormatModule, FCTimeFormatModule } from 'src/@common/directives/fc-directive.module';
import { FCImageLoaderModule } from 'src/@common/components/image-loader/fc-image-loader.module';

@NgModule({
  declarations: [LeadPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCValidatorModule,
    FCDateFormatModule,
    FCTimeFormatModule,
    FCDecimalFormatModule,
    FCImageLoaderModule
  ],
  exports: [LeadPage]
})
export class LeadPageModule { }
