import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ClientManagementTransactionHistoryComponent } from './client-management-detail-transaction-history.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';

@NgModule({
    declarations: [ClientManagementTransactionHistoryComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        PGridViewModule
    ],
    exports: [ClientManagementTransactionHistoryComponent]
})
export class ClientManagementTransactionHistoryModule { }
