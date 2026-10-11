import { Injectable, OnDestroy } from '@angular/core';
import { Camera, CameraOptions, PictureSourceType } from '@ionic-native/camera/ngx';
import { FCFileInfo } from 'src/@common/utils';
import { FCFileTransferService } from './fc-file-transfer.service';

@Injectable()
export class FCCameraService implements OnDestroy {

    readonly maxFileSize: number = 4 * 1024 * 1024;//4MB
    constructor(private _camera: Camera,
        private _fileTransfer: FCFileTransferService) {
    }

    ngOnDestroy(): void {
        this._fileTransfer.destory();
    }

    takePhoto(param: { uploadId: string, fileType: number, sourceType: PictureSourceType }): Promise<FCFileInfo> {

        return new Promise((resolve, reject) => {
            const options: CameraOptions = {
                quality: 100,
                targetWidth: 900,
                targetHeight: 600,
                destinationType: this._camera.DestinationType.DATA_URL,
                encodingType: this._camera.EncodingType.JPEG,
                mediaType: this._camera.MediaType.PICTURE,
                sourceType: param.sourceType,
                correctOrientation: true,
                cameraDirection: 1
            }

            this._camera.getPicture(options)
                .then((result) => {
                    if (this.isValidSize(result)) {
                        this._fileTransfer.uploadBase64({
                            path: `data:image/jpeg;base64,${result}`,
                            fileName: 'photo.jpeg',
                            fileType: param.fileType,
                            uploadId: param.uploadId,
                            mediaType: 'image/jpeg'
                        }).then((response) => {
                            resolve(response);
                        }).catch((error) => reject(error))
                    }
                    else {
                        reject('Maximum file size should not be exceeded 4 MB.');
                    }
                }).catch((error) => reject(error))
        })
    }

    private isValidSize(base64: string) {
        return this.getSize(base64) < this.maxFileSize;
    }

    private getSize(base64: string) {
        return Math.round((3 / 4) * base64.length)
    }
}
