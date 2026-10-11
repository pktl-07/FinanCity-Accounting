import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PImageUploaderComponent } from './p-image-uploader.component';
import { UploadModule } from '@progress/kendo-angular-upload';
import { LoadingMoule } from '../p-loading/p-loading.module';
import { PConfirmDialogModule } from '../p-confirm-dialog/p-confirm-dialog.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from '@progress/kendo-angular-buttons';

@NgModule({
    declarations: [PImageUploaderComponent],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        CommonModule,
        UploadModule,
        LoadingMoule,
        ButtonModule,
        PConfirmDialogModule
    ],
    exports: [PImageUploaderComponent],
})
export class PImageUploaderModule { }
