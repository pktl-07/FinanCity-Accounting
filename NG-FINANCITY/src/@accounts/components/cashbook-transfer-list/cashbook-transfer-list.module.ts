import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { CashbookTransferListComponent } from './cashbook-transfer-list.component';
import { CashbookTransferDetailModule } from '../cashbook-transfer-detail/cashbook-transfer-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CashbookTransferListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    CashbookTransferDetailModule,
    TextBoxModule,
    RouterModule.forChild([{ path: '', component: CashbookTransferListComponent }])
  ]
})
export class CashbookTransferListModule { }
