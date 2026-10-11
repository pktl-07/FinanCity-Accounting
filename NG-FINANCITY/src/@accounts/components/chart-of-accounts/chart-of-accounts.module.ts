import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartOfAccountComponent } from './chart-of-accounts.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { GridModule } from '@progress/kendo-angular-grid';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ChartOfAccountGridComponent } from './chart-of-accounts-grid.component';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { AccountSetupDetailModule } from '../account-setup-detail/account-setup-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ChartOfAccountComponent,
    ChartOfAccountGridComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
    PModalFormModule,
    PToolbarModule,
    GridModule,
    LayoutModule,
    PWindowModule,
    AccountSetupDetailModule,
    RouterModule.forChild([{ path: '', component: ChartOfAccountComponent }])
  ],
  exports: [ChartOfAccountComponent,
    ChartOfAccountGridComponent]
})
export class ChartOfAccountModule { }
