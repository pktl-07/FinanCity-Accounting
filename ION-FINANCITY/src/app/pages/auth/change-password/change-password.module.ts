import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { ChangePasswordPage } from './change-password.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { FCNotificationComponentModule } from '../../shared/notification/notification-component-module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ReactiveFormsModule,
    RouterModule.forChild([{ path: '', component: ChangePasswordPage }]),
    FCValidatorModule,
    FCNotificationComponentModule,
  ],
  declarations: [ChangePasswordPage]
})
export class ChangePasswordPageModule { }
