import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountingPeriodComponent } from './accounting-peroid.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { GridModule } from '@progress/kendo-angular-grid';
import { PDateFormatPipeModule } from 'src/@common/pipe/p-date-format-pipe/p-date-format-pipe-module';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';
import { AccountingPeriodCreateModule } from './accounting-period-create/accounting-period-create.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [AccountingPeriodComponent],
  imports: [
    CommonModule,
    ButtonModule,
    FormsModule,
    ReactiveFormsModule,
    DropDownsModule,
    GridModule,
    PToolbarModule,
    PDateFormatPipeModule,
    PConfirmDialogModule,
    AccountingPeriodCreateModule,
    RouterModule.forChild([{ path: '', component: AccountingPeriodComponent }])
  ],
  exports: [AccountingPeriodComponent]
})
export class AccountingPeriodModule { }
