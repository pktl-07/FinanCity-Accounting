import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PSalespersonPipeModule } from 'src/@common/pipe/p-salesperson-pipe/p-salesperson-pipe-module';
import { LeadManagementRemarkComponent } from './lead-management-detail-remarks.component';
import { PFilePipeModule } from 'src/@common/pipe/p-file-pipe/p-file-pipe-module';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';
import { PDateFormatPipeModule } from 'src/@common/pipe/p-date-format-pipe/p-date-format-pipe-module';

const components = [
    LeadManagementRemarkComponent
];

@NgModule({
    entryComponents: [components],
    declarations: [components],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        PSalespersonPipeModule,
        PFilePipeModule,
        PConfirmDialogModule,
        PDateFormatPipeModule
    ],
    exports: [components]
})
export class LeadManagementRemarkModule { }
