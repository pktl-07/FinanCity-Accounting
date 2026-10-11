import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResetPasswordComponent } from './reset-password.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ResetPasswordComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    PConfirmDialogModule,
    RouterModule.forChild([{ path: '', component: ResetPasswordComponent }])
  ]
})
export class ResetPasswordModule { }
