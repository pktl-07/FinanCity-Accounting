import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { FileService } from 'src/@common/service/file.service';
import { SubSink } from 'src/@common/service/utility-service';

@Pipe({
    name: 'pSalespersonImage',
})
export class PSalespersonImagePipe implements PipeTransform, OnDestroy {

    private _subsink = new SubSink();
    private _listeners = [];

    constructor(private _fileService: FileService) {
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
        this._listeners.forEach((listener) => {
            listener()
        })
    }

    transform(value: any, ...fileType: number[]): any {

        if (typeof fileType == 'undefined')
            fileType = [1];

        if (value === undefined || value === null)
            return null;
        else if (value === '1')
            return this.getFile({ fileType: 10, uploadId: value });
        else
            return this.getFile({ fileType: fileType[0], uploadId: value });
    }

    private getFile(param: { fileType: number, uploadId: string }) {

        return new Promise(async (resolve) => {

            const reader = new FileReader();
            const listener = reader.onload = () => {
                this._listeners.push(listener);
                resolve(<string>reader.result);
            }

            this._subsink.sink = this._fileService.file({ fileType: param.fileType, uploadId: param.uploadId })
                .subscribe((file) => {
                    reader.readAsDataURL(file);
                })
        })
    }
}