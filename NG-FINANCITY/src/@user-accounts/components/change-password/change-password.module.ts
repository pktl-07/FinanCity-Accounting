import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChangePasswordComponent } from './change-password.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ChangePasswordComponent],
  imports: [
    CommonModule,
    TextBoxModule,
    ReactiveFormsModule,
    FormsModule,
    PConfirmDialogModule,
    RouterModule.forChild([{ path: '', component: ChangePasswordComponent }])
  ]
})
export class ChangePasswordModule { }
