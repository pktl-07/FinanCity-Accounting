import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, map, mergeMap, finalize, takeUntil } from 'rxjs/operators';
import { getBlobFileName } from './utility-service';
import { BehaviorSubject, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class FileService implements OnDestroy {

    private _subscriber = new Subject();
    loading = new BehaviorSubject(false);

    constructor(public http: HttpClient) {
    }

    ngOnDestroy(): void {

        this._subscriber.next();
        this._subscriber.complete();
    }

    upload(param: { file: FileList, fileType: number, uploadId: string }) {

        this.loading.next(true);
        const fd = new FormData();
        const headers = new HttpHeaders(this.getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        const f: File = param.file[0];

        fd.append("File", f, f.name);
        return this.http.post(`${environment.auth.api}/File/upload`,
            fd, { headers: headers })
            .pipe(
                takeUntil(this._subscriber),
                finalize(() => this.loading.next(false)));
    }

    delete(param: { fileType: number, uploadId: string }) {

        this.loading.next(true);
        let headers = new HttpHeaders(this.getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this.http.delete(`${environment.auth.api}/File/delete/${param.fileType}/${param.uploadId.toString()}`, { headers: headers })
            .pipe(
                takeUntil(this._subscriber),
                finalize(() => this.loading.next(false)));
    }

    download(param: { fileType: number, uploadId: string }) {

        this.loading.next(true);
        let headers = new HttpHeaders(this.getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));

        return this.http.get<Blob>(`${environment.auth.api}/File/get/${param.fileType}/${param.uploadId}`, {
            observe: 'response',
            responseType: 'blob' as 'json',
            headers: headers
        }).pipe(
            takeUntil(this._subscriber),
            tap(response => {
                let fileName: string = getBlobFileName(response);
                let bindaryData = [];

                bindaryData.push(response.body);

                let downloadLink = document.createElement('a');
                downloadLink.href = window.URL.createObjectURL(new Blob(bindaryData, { type: 'blob' }));
                downloadLink.setAttribute('download', fileName);
                document.body.appendChild(downloadLink);
                downloadLink.click();
            }),
            finalize(() => this.loading.next(false))
        )
    }

    file(param: { fileType: number, uploadId: string }) {

        this.loading.next(true);
        let headers = new HttpHeaders(this.getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));

        return this.http.get<Blob>(`${environment.auth.api}/File/get/${param.fileType}/${param.uploadId}`, {
            observe: 'response',
            responseType: 'blob' as 'json',
            headers: headers
        }).pipe(
            takeUntil(this._subscriber),
            map((response) => {

                const c = {};
                c['file'] = response;
                return c;
            }),
            mergeMap((response) => {

                return this.info({ fileType: param.fileType, uploadId: param.uploadId })
                    .pipe(map(data => {
                        response['mime'] = data['Mime']
                        return response;
                    }))
            }),
            map(response => {

                let fileName: string = getBlobFileName(response['file']);
                let bindaryData = [];

                bindaryData.push(response['file'].body);
                let blob = new Array<BlobPart>();
                blob.push(new Blob(bindaryData, { type: 'blob' }));

                return new File(blob, fileName, { type: response['mime'] });
            }),
            finalize(() => this.loading.next(false))
        )
    }

    url(param: { fileType: number, uploadId: string }) {

        this.loading.next(true);
        let headers = new HttpHeaders(this.getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this.http.get(`${environment.auth.api}/File/url/${param.fileType}/${param.uploadId.toString()}`,
            { headers: headers })
            .pipe(
                takeUntil(this._subscriber),
                finalize(() => this.loading.next(false)));
    }

    has(param: { fileType: number, uploadId: string }) {

        this.loading.next(true);
        let headers = new HttpHeaders(this.getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this.http.get(`${environment.auth.api}/File/has/${param.fileType}/${param.uploadId.toString()}`,
            { headers: headers })
            .pipe(
                takeUntil(this._subscriber),
                finalize(() => this.loading.next(false)));
    }

    info(param: { fileType: number, uploadId: string }) {

        this.loading.next(true);
        let headers = new HttpHeaders(this.getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this.http.get(`${environment.auth.api}/File/info/${param.fileType}/${param.uploadId.toString()}`,
            { headers: headers })
            .pipe(
                takeUntil(this._subscriber),
                finalize(() => this.loading.next(false)));
    }

    path(param: { fileType: number }) {

        let folder: string;
        switch (param.fileType) {

            case 1:
                folder = 'People';
                break;
            case 5:
                folder = 'BankBestRate';
            case 10:
                folder = 'Company';
                break;
            case 15:
                folder = 'Lead';
                break;
        }
        return `${environment.auth.api}/App_Data/Uploads/${folder}`;
    }

    private getHeaderScheme(expression?: {}) {

        let headerJson = {
            'authorize': 'authorize'
        };

        if (typeof expression != "undefined" || expression != null) {
            Object.getOwnPropertyNames(expression).forEach(f => {
                if (expression[f] !== undefined) {
                    headerJson[f.toString()] = expression[f].toString();
                }
            });
        }
        return headerJson;
    }
}