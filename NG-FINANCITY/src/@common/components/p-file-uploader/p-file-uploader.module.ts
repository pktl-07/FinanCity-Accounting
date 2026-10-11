import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UploadModule } from '@progress/kendo-angular-upload';
import { LoadingMoule } from '../p-loading/p-loading.module';
import { PFileUploaderComponent } from './p-file-uploader.component';
import { UploadsModule } from '@progress/kendo-angular-upload';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PConfirmDialogModule } from '../p-confirm-dialog/p-confirm-dialog.module';
import { ButtonModule } from '@progress/kendo-angular-buttons';

@NgModule({
    declarations: [PFileUploaderComponent],
    imports: [
        CommonModule,
        UploadModule,
        UploadsModule,
        LoadingMoule,
        ButtonModule,
        ReactiveFormsModule,
        FormsModule,
        PConfirmDialogModule,
    ],
    exports: [PFileUploaderComponent],
})
export class PFileUploaderModule { }
