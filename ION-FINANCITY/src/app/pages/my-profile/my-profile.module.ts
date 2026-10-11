import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { MyProfilePage } from './my-profile.page';
import { SuperTabsModule } from '@ionic-super-tabs/angular';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { MyProfilePersonPage } from './personal.page';
import { ContactPageModule } from '../shared/contact/contact.module';
import { MyProfilBankPage } from './bank.page';
import { MyProfilEmploymentPage } from './employment.page';
import { PayNowPage } from './my-profile.paynow.page';
import { FCImageLoaderModule } from 'src/@common/components/image-loader/fc-image-loader.module';
import { FCCurrencyFormatModule, FCDateFormatModule } from 'src/@common/directives/fc-directive.module';

@NgModule({
  declarations: [MyProfilePage,
    MyProfilePersonPage,
    MyProfilBankPage,
    MyProfilEmploymentPage,
    PayNowPage],
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ReactiveFormsModule,
    SuperTabsModule,
    FCValidatorModule,
    ContactPageModule,
    FCImageLoaderModule,
    FCDateFormatModule,
    FCCurrencyFormatModule,
    RouterModule.forChild([{ path: '', component: MyProfilePage }]),
  ],
  exports: [MyProfilePage,
    MyProfilePersonPage,
    MyProfilBankPage,
    MyProfilEmploymentPage,
    PayNowPage]

})
export class MyProfilePageModule { }
