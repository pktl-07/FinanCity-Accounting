import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { FileService } from 'src/@common/service/file.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { FileSelectComponent } from '@progress/kendo-angular-upload';
import { PConfirmDialogComponent } from '../p-confirm-dialog/p-confirm-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { PFileDeletWarnningComponent } from '../p-confirm-dialog/p-error-component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
    selector: 'p-file-loader',
    template: `
    <div [formGroup]='form'>
        <kendo-fileselect formControlName="file"
            [multiple]="false" 
            (valueChange)="handleValueChange($event)">
        </kendo-fileselect>
        <ng-container *ngIf="showDownloadButton">
            <p class="k-block" style="background-color: #787878; color: #ffffff;">
                <button kendoButton look="flat" icon="download" (click)="handleDownload()">Download</button>
                <button kendoButton look="flat" icon="close" class="float-right" (click)="handleRemove()">Remove</button>
            </p>
        </ng-container>
        <div *ngIf="_fileService.loading | async" p-loading></div>
        <confirm-dialog #confirmDialog></confirm-dialog>
    </div>
    `,
    providers: [
        FileService
    ]
})
export class PFileUploaderComponent implements OnInit, OnDestroy {

    @Input() uploadId: string;
    @Input() fileType: number;
    @ViewChild(FileSelectComponent, { static: true }) public _fileUploader: FileSelectComponent;
    @ViewChild('confirmDialog', { static: true }) confirmDialog: PConfirmDialogComponent;

    showDownloadButton: boolean = false;
    form: FormGroup;

    private _subsink = new SubSink();
    constructor(public _fileService: FileService,
        private _fb: FormBuilder) {

        this.form = this._fb.group({
            file: null
        })

        this._subsink.sink = this.form.get('file')
            .valueChanges
            .subscribe((file) => {

                const c = file != null;
                this._fileUploader.setDisabledState(c);
                this.showDownloadButton = c;
            })
    }

    ngOnInit(): void {

        this._subsink.sink = this._fileService.info({
            fileType: this.fileType,
            uploadId: this.uploadId
        }).subscribe((response: any) => {
            this.form.get('file').patchValue([{
                name: response.Name,
                size: response.Size,
            }]);
        })
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    handleValueChange(file: any) {

        if (typeof this.uploadId == 'undefined' ||
            typeof this.fileType == 'undefined' ||
            this.uploadId == null ||
            this.fileType == null) {

            alert(`uploadId or fileType property should have some value assign.`);

        } else {

            this._subsink.sink = this._fileService.upload({
                file: file,
                fileType: this.fileType,
                uploadId: this.uploadId
            }).subscribe();
        }
    }

    handleDownload() {

        this._subsink.sink = this._fileService.download({
            fileType: this.fileType,
            uploadId: this.uploadId
        }).subscribe();
    }

    clear() {

        this.form.get('file').patchValue(null);
    }

    handleRemove() {

        const ref: DialogRef = this.confirmDialog.open({
            title: 'Warning',
            content: PFileDeletWarnningComponent,
            action: [{ text: 'OK' }, { text: 'Cancel' }]
        });

        this._subsink.sink = ref.result
            .subscribe((result: any) => {

                if (result.text == 'OK') {

                    this._subsink.sink = this._fileService.delete({
                        fileType: this.fileType,
                        uploadId: this.uploadId
                    }).subscribe(() => {

                        this.form.get('file').patchValue(null);
                    });
                }
            })
    }
}
