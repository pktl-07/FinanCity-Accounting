import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ExternalCobrokeCreateModalPage } from './external-cobroke-create-modal.page';
import { ContactPageModule } from '../contact/contact.module';
import { ExternalCobrokeModule } from '../external-cobroke/external-cobroke.module';

@NgModule({
  entryComponents: [ExternalCobrokeCreateModalPage],
  declarations: [ExternalCobrokeCreateModalPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    ExternalCobrokeModule,
    ContactPageModule,
  ],
  exports: [ExternalCobrokeCreateModalPage]
})
export class ExternalCobrokeCreateModalPageModule { }
