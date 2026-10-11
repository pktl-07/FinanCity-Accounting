import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { FileService } from '../components/native-devices/fc-file-service';
import { SubSink } from '../utils';

@Pipe({
    name: 'fcImage',
})
export class FCImagePipe implements PipeTransform, OnDestroy {

    public _subsink = new SubSink();

    constructor(private _fileService: FileService) {
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    transform(value: any, ...fileType: any[]): any {

        if (typeof fileType === 'undefined')
            fileType = [1];

        if (typeof value === 'undefined' || value === null)
            return null;

        else if (value === '1')
            return this.getFile({ fileType: 10, uploadId: value });
        else
            return this.getFile({ fileType: fileType[0], uploadId: value });
    }

    private getFile(param: { fileType: number, uploadId: string }) {

        return new Promise(async (resolve) => {
            this._subsink.sink = this._fileService.url({ fileType: param.fileType, uploadId: param.uploadId })
                .subscribe((result) => {
                    resolve(result);
                })
        })
    }
}