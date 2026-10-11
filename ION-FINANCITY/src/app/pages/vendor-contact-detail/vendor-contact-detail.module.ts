import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { VendorContactDetailPage } from './vendor-contact-detail.page';
import { SuperTabsModule } from '@ionic-super-tabs/angular'
import { ContactPageModule } from '../shared/contact/contact.module';
import { CompanyPageModule } from '../shared/company/company.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    SuperTabsModule,
    CompanyPageModule,
    ContactPageModule,
    RouterModule.forChild([{ path: '', component: VendorContactDetailPage }]),
  ],
  declarations: [VendorContactDetailPage]
})
export class VendorContactDetailPageModule { }
