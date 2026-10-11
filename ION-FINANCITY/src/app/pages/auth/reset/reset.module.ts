import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { ResetPage } from './reset.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    RouterModule.forChild([{ path: '', component: ResetPage }]),
    ReactiveFormsModule,
    FCValidatorModule
  ],
  declarations: [ResetPage]
})
export class ResetPageModule { }
