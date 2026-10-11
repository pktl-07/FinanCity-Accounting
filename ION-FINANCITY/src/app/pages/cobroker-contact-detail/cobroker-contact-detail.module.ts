import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { CoBrokerContactDetailPage } from './cobroker-contact-detail.page';
import { SuperTabsModule } from '@ionic-super-tabs/angular'
import { ExternalCobrokeModule } from '../shared/external-cobroke/external-cobroke.module';
import { ContactPageModule } from '../shared/contact/contact.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    SuperTabsModule,
    ExternalCobrokeModule,
    ContactPageModule,
    RouterModule.forChild([{ path: '', component: CoBrokerContactDetailPage }]),
  ],
  declarations: [CoBrokerContactDetailPage]
})
export class CoBrokerContactDetailPageModule { }
