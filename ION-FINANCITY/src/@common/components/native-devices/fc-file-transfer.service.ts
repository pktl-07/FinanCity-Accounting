import { Injectable, OnDestroy } from '@angular/core';
import { Storage } from '@ionic/storage';
import { FCFileInfo, SubSink } from 'src/@common/utils';
import { File, FileEntry } from '@ionic-native/file/ngx';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { FilePath } from '@ionic-native/file-path/ngx';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class FCFileTransferService implements OnDestroy {

    private _subsink = new SubSink();
    private _listeners = [];

    constructor(
        public _http: HttpClient,
        private _nativeStorage: Storage,
        private _filePath: FilePath,
        private _file: File) {
    }

    destory() {
        this._subsink.unsubscribe();
        this._listeners.forEach((file) => {
            file();
        })
    }

    ngOnDestroy(): void {
        this.destory();
    }

    public uploadBase64(param: {
        path: string,
        uploadId: string,
        fileName: string,
        fileType: number,
        mediaType?: string
    }): Promise<FCFileInfo> {

        return new Promise(async (resolve, reject) => {

            if (typeof param.mediaType == 'undefined')
                param.mediaType = this.mime(param.path);

            const blob = this.toBlob(param.path);
            const formData = new FormData();

            formData.append('File', await blob, param.fileName);

            this.uploadFile(formData, {
                b64: param.path,
                fileType: param.fileType,
                uploadId: param.uploadId
            })
                .then((result) => resolve(result))
                .catch((error) => reject(error));
        })
    }

    async toBlob(b64: string): Promise<Blob> {

        return new Promise((resolve, reject) => {
            fetch(b64).then((base64Response) => {
                base64Response.blob().then((blob) => {

                    resolve(blob)
                })
                    .catch(() => reject('error occur while converting to Blob.'))
            }).catch(() => reject('error occur while producing the base64Response.'))
        })
    }

    loadFileURL(param: { path: string, uploadId: string, fileType: number }): Promise<FCFileInfo> {

        return new Promise(async (resolve, reject) => {

            this._filePath.resolveNativePath(param.path).then((path) => {
                this._file.resolveLocalFilesystemUrl(path)
                    .then((entry: any) => {

                        (<FileEntry>entry).file((file) => {

                            const reader = new FileReader();
                            const listener = reader.onload = () => {

                                const formData = new FormData();
                                const blob = new Blob([reader.result], { type: file.type })

                                formData.append('File', blob, file.name);

                                const uri = this.splitUri(param.path);
                                this._file.readAsDataURL(uri.dirpath, uri.fileName)
                                    .then((base64Data) => {

                                        this.uploadFile(formData, {
                                            b64: base64Data,
                                            fileType: param.fileType,
                                            uploadId: param.uploadId
                                        })
                                            .then((result) => resolve(result))
                                            .catch((error) => reject(error));
                                    })
                                    .catch(() => reject('error occur while reading data uri.'))
                            }

                            reader.readAsArrayBuffer(file);
                            this._listeners.push(listener);
                        })
                    }).catch(() => reject('error occur while resolve local file system url.'))
            }).catch(() => reject('error occur while resolve native file path.'))
        })
    }

    download(param: { fileType: number, uploadId: string }): Promise<FileEntry> {

        return new Promise(async (resolve, reject) => {

            const headers = await this.getHttpHeaders(param);
            this._subsink.sink = this._http.get(`${environment.auth.api}/File/get/${param.fileType}/${param.uploadId}`, {
                observe: 'response',
                responseType: 'blob',
                headers: headers
            }).pipe(
                map(response => {
                    const blob = new Blob([response.body], { type: 'blob' });
                    const result: { fileName: string, file: Blob } = {
                        file: blob,
                        fileName: this.getBlobFileName(response),
                    }
                    return result;
                })).subscribe((response) => {
                    this._file.writeFile(this._file.externalDataDirectory, response.fileName, response.file, { replace: true })
                        .then((entry) => {
                            resolve(entry);
                        })
                        .catch(() => reject('error occur creating with file.'))
                }, (err) => reject(err))
        })
    }

    private uploadFile(formData: FormData,
        param: { b64: string, uploadId: string, fileType: number }): Promise<FCFileInfo> {

        return new Promise(async (resolve, reject) => {

            const headers = await this.getHttpHeaders(param);
            this._subsink.sink = this._http.post(`${environment.auth.api}/File/upload`,
                formData, { headers: headers })
                .subscribe((response: any) => {
                    resolve({
                        contentType: this.mime(param.b64),
                        description: null,
                        uploadId: param.uploadId,
                        name: response.Name,
                        fileType: param.fileType.toString(),
                        size: response.Size,
                        url: param.b64,
                    })
                }, (err) => reject(err))
        })
    }

    private getHttpHeaders(param: { uploadId: string, fileType: number }): Promise<HttpHeaders> {

        return new Promise(async (resolve) => {

            const token = await this._nativeStorage.get('userToken');
            const headers = new HttpHeaders({
                authorize: 'authorize',
                Authorization: `Bearer ${token}`,
                fileType: param.fileType.toString(),
                uploadId: param.uploadId.toString()
            });
            resolve(headers);
        })
    }

    private getBlobFileName(response: HttpResponse<Blob>) {

        try {

            const contentDisposition: string = response.headers.get('Content-Disposition');
            const r = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
            return r.exec(contentDisposition)[1];

        } catch (e) {

            return 'myfile.txt';
        }
    }


    private mime(base64: string) {
        return base64.match(/data:([a-zA-Z0-9]+\/[a-zA-Z0-9-.+]+).*,.*/)[1];
    }

    private splitUri(uri: string) {

        return {
            fileName: uri.split('/').pop(),
            dirpath: uri.substring(0, uri.lastIndexOf("/") + 1)
        }
    }
}
