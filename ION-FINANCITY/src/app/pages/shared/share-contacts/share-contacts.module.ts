import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { ShareContactsPage } from './share-contacts.page';
import { FCImagePipeModule } from 'src/@common/pipes/fc.pipe.module';

@NgModule({
  declarations: [ShareContactsPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCImagePipeModule
  ],
  exports: [ShareContactsPage]
})
export class ShareContactsModule { }
