import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountYearClosingComponent } from './year-closing.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';
import { RouterModule } from '@angular/router';

@NgModule({
  entryComponents: [AccountYearClosingComponent],
  declarations: [AccountYearClosingComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DropDownsModule,
    DialogModule,
    ButtonModule,
    PConfirmDialogModule,
    RouterModule.forChild([{ path: '', component: AccountYearClosingComponent }])
  ],
  exports: [AccountYearClosingComponent]
})
export class YearClosingModule { }
