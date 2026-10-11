import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { HomePage } from './home.page';
import { FCNotificationComponentModule } from '../shared/notification/notification-component-module';

@NgModule({
  declarations: [HomePage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCNotificationComponentModule,
    RouterModule.forChild([{ path: '', component: HomePage }]),
  ],
})
export class HomePageModule { }
