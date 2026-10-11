import { Injectable, OnDestroy } from '@angular/core';
import { FileOpener } from '@ionic-native/file-opener/ngx';
import { SubSink } from 'src/@common/utils';
import { FileService } from '../native-devices/fc-file-service';
import { FCFileTransferService } from '../native-devices/fc-file-transfer.service';

@Injectable({ providedIn: 'root' })
export class FCFileOpenerService implements OnDestroy {

    private _subsink = new SubSink();
    constructor(private _fileOpener: FileOpener,
        private _fileService: FileService,
        private _fileTransfer: FCFileTransferService) {
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
        this._fileTransfer.destory();
    }

    isDownloadAvailable(param: { fileType: number, uploadId: string }): Promise<boolean> {
        return new Promise((resolve) => {
            this.fetchFileInfo(param)
                .then((file) => resolve(file != null))
        })
    }

    openDownload(param: { fileType: number, uploadId: string }): Promise<any> {

        return new Promise((resolve, reject) => {

            this.fetchFileInfo(param)
                .then((info) => {
                    this._fileTransfer.download({ fileType: param.fileType, uploadId: param.uploadId })
                        .then((entry) => {
                            this._fileOpener.open(entry.nativeURL, info.mime)
                                .then(() => resolve(entry))
                                .catch(() => reject(`Your device can not open ${info.extension} file. You should search and install associate APK to open ${info.extension} from the Google Play Store.`))

                        }).catch(() => reject('Error while downloading file.'))
                })
        })
    }

    private fetchFileInfo(param: { fileType: number, uploadId: string })
        : Promise<{ mime: string, fileName: string, extension: string }> {

        return new Promise((resolve, reject) => {

            this._subsink.sink = this._fileService.info({
                fileType: param.fileType,
                uploadId: param.uploadId
            }).subscribe((file: any) => {
                if (file != null) {
                    resolve({
                        mime: file.Mime,
                        fileName: file.Name,
                        extension: file.Extension
                    })
                }
            }, (error) => reject(error))
        })
    }
}