import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CompanyCreateModalPage } from './company-create-modal.page';
import { CompanyPageModule } from '../company/company.module';
import { ContactPageModule } from '../contact/contact.module';
import { FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  entryComponents: [CompanyCreateModalPage],
  declarations: [CompanyCreateModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    CompanyPageModule,
    ContactPageModule,
  ],
  exports: [CompanyCreateModalPage]
})
export class CompanyCreateModalPageModule { }
