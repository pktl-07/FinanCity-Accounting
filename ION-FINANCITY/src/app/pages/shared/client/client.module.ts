import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ClientPage } from './client.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { FCDateFormatModule } from 'src/@common/directives/fc-directive.module';
import { FCImageLoaderModule } from 'src/@common/components/image-loader/fc-image-loader.module';

@NgModule({
  declarations: [ClientPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCValidatorModule,
    FCDateFormatModule,
    FCImageLoaderModule
  ],
  exports: [ClientPage]
})
export class ClientPageModule { }
