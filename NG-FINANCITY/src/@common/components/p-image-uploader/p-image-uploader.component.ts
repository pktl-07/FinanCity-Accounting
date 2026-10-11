import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { FileService } from 'src/@common/service/file.service';
import { PConfirmDialogComponent } from '../p-confirm-dialog/p-confirm-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { PFileDeletWarnningComponent } from '../p-confirm-dialog/p-error-component';
import { FormGroup, FormBuilder } from '@angular/forms';
import { filter, mergeMap } from 'rxjs/operators';
import { ToastService } from '../p-toast/p-toast.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
    selector: 'p-image-loader',
    template: `
    <div class="image-uploader-container">
        <div class="text-danger img-thumbnail" *ngIf="isLarge">
            Maximum file size should not be exceeded <b>4 MB</b>.
            <button kendoButton (click)="handleOK()">Try again</button>
        </div>
    </div>
    <div *ngIf="isLarge == false" class="image-uploader-container" [style.width.px]='width' [style.height.px]='height'>
        <input type="file" accept="image/*" name="saveImage" id="saveImage" (change)="handleSelect($event)"
            style="display:none" #inputFile />
        <div class="image-uploader-button">
            <div class="float-left" *ngIf="!hasFile">
                <button type="file" accept="image/*" class="btn btn-outline" (click)="inputFile.click()">
                <i style="color: var(--light);" class="fal fa-pencil"></i>
                </button>
            </div>
            <div class="float-right" *ngIf="hasFile">
                <button class="btn btn-outline" (click)="handleRemove()">
                    <i style="color: var(--light);" class="fal fa-trash-alt"></i>
                </button>
            </div>
        </div>
        <img class="img-thumbnail" [src]="form?.get('file')?.value" onerror="this.src='assets/imgs/empty-picture.png'">
        <div *ngIf="_fileService.loading | async" p-loading></div>
    </div>
    <confirm-dialog #confirmDialog></confirm-dialog>
    `,
    styles: [
        `
            .image-uploader-container{
                position: absolute;
            }

            .image-uploader-container:hover 
            .image-uploader-button{
                display: block;
            }  

            .image-uploader-container 
            .image-uploader-button{
                position: absolute;
                width:100%;
                background: var(--secondary);
                opacity: 0.7;   
                display: none;
            }
        `
    ],
    providers: [FileService, ToastService]
})
export class PImageUploaderComponent implements OnInit, OnDestroy {

    @Input() uploadId: string;
    @Input() fileType: number;
    @Input() width: number = 100;
    @Input() height: number = 100;

    @ViewChild('confirmDialog', { static: true }) confirmDialog: PConfirmDialogComponent;

    hasFile: boolean = false;
    form: FormGroup;
    loading: boolean = false;
    reader: FileReader;
    isLarge: boolean = false;
    maxFileSize: number = 4 * 1024 * 1024;

    private _listeners = [];

    public _subsink = new SubSink()
    constructor(public _fileService: FileService,
        private _toastService: ToastService,
        _formBuilder: FormBuilder) {

        this.reader = new FileReader();
        this.form = _formBuilder.group({
            file: null,
        })

        const listener = this.reader.onload = () => {
            this.form.get('file').patchValue(<string>this.reader.result);
            this._listeners.push(listener);
        }

        this._subsink.sink = this.form.get('file').valueChanges
            .subscribe((value) => {
                this.hasFile = value != null;
            })
    }

    ngOnInit(): void {

        this._subsink.sink = this._fileService.has({ fileType: this.fileType, uploadId: this.uploadId })
            .subscribe((h: boolean) => {

                if (h) {
                    this._subsink.sink = this._fileService.file({ fileType: this.fileType, uploadId: this.uploadId })
                        .subscribe((response: any) => {
                            this.reader.readAsDataURL(response);
                        })
                }
            })
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
        this._listeners.forEach(listener => {
            listener();
        });
    }

    handleRemove() {

        const ref: DialogRef = this.confirmDialog.open({
            title: 'Warning',
            content: PFileDeletWarnningComponent,
            action: [{ text: 'OK' }, { text: 'Cancel' }]
        });

        this._subsink.sink = ref.result.pipe(
            filter((result: any) => result.text == 'OK'),
            mergeMap(() => {
                return this._fileService.delete({
                    fileType: this.fileType,
                    uploadId: this.uploadId
                })
            })
        ).subscribe(() => this.clear(),
            (error) => this._toastService.open({
                data: {
                    title: 'Error',
                    message: 'Could not delete the picture image, please contact our Administrator.'
                }
            }))
    }

    handleOK() {
        this.isLarge = false;
    }

    clear() {

        this.form.get('file').patchValue(null);
    }

    handleSelect(event: any) {

        let selectedFile = <FileList>event.target.files;
        if (selectedFile[0].size > this.maxFileSize) {
            this.isLarge = true;
        } else {
            this._subsink.sink = this._fileService.upload({ file: selectedFile, fileType: this.fileType, uploadId: this.uploadId })
                .subscribe(() => {

                    this.reader.readAsDataURL(selectedFile[0]);
                })
        }
    }
}
