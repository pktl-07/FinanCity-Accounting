import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/utils';
import { map, mergeMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class FileService {

    constructor(private _http: HttpClient) {
    }

    info(param: { fileType: number, uploadId: string }) {

        let headers = new HttpHeaders(getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this._http.get(`${environment.auth.api}/File/info/${param.fileType}/${param.uploadId}`,
            { headers: headers })
    }

    delete(param: { fileType: number, uploadId: string }) {

        let headers = new HttpHeaders(getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this._http.delete(`${environment.auth.api}/File/delete/${param.fileType}/${param.uploadId.toString()}`, { headers: headers })
    }

    has(param: { fileType: number, uploadId: string }) {

        let headers = new HttpHeaders(getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this._http.get(`${environment.auth.api}/File/has/${param.fileType}/${param.uploadId.toString()}`,
            { headers: headers })
    }

    get(param: { fileType: number, uploadId: string }) {

        let headers = new HttpHeaders(getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this._http.get(`${environment.auth.api}/File/get/${param.fileType}/${param.uploadId}`, {
            observe: 'response',
            responseType: 'blob' as 'json',
            headers: headers
        }).pipe(
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

                let bindaryData = [];
                bindaryData.push(response['file'].body);
                return `data:${response['mime']};base64,${response['file'].body}`;
            }))
    }

    url(param: { fileType: number, uploadId: string }) {

        let headers = new HttpHeaders(getHeaderScheme({ fileType: param.fileType, uploadId: param.uploadId }));
        return this._http.get(`${environment.auth.api}/File/url/${param.fileType}/${param.uploadId}`, {
            headers: headers
        })
    }
}