import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ClientCreateModalPage } from './client-create-modal.page';
import { ClientPageModule } from '../client/client.module';
import { ContactPageModule } from '../contact/contact.module';

@NgModule({
  entryComponents: [ClientCreateModalPage],
  declarations: [ClientCreateModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    ClientPageModule,
    ContactPageModule,
  ],
  exports: [ClientCreateModalPage]
})
export class ClientCreateModalPageModule { }
