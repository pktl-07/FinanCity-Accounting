import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ContactPage } from './contact.page';
import { CountryListModalPageModule } from '../country-list-modal/country-list-modal.module';
import { FCModalSelectModule } from 'src/@common/components/modal-select/modal-select.module';

@NgModule({
  entryComponents: [ContactPage],
  declarations: [ContactPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    CountryListModalPageModule,
    FCModalSelectModule
  ],
  exports: [ContactPage]
})
export class ContactPageModule { }
