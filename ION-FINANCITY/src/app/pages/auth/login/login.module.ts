import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { LoginPage } from './login.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';

@NgModule({
  imports: [
    CommonModule,
    IonicModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild([{ path: '', component: LoginPage }]),
    FCValidatorModule
  ],
  declarations: [LoginPage]
})
export class LoginPageModule { }
