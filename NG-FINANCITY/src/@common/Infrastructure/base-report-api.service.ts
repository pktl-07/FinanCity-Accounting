import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from './common-interface';
import { State } from '@progress/kendo-data-query';
import { Directive, OnDestroy } from '@angular/core';
import { getBlobFileName, SubSink } from '../service/utility-service';
import { tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Directive()
export class BaseReportApiService implements OnDestroy {

    public loading: boolean;

    private _http: HttpClient;
    private _route: string;
    protected subsink = new SubSink();

    constructor(http: HttpClient,
        route: string) {

        this._route = route;
        this._http = http;
    }

    ngOnDestroy(): void {
        this.subsink.unsubscribe();
    }

    get(path: string, state: State) {

        this.loading = true;

        let headers = new HttpHeaders(getHeaderScheme({
            'expression': JSON.stringify(state.filter)
        }));

        return this._http.get<Blob>(`${environment.auth.api}/${this._route}/${path}`, {
            observe: 'response',
            responseType: 'blob' as 'json',
            headers: headers
        }).pipe(
            tap(response => {
                let fileName: string = getBlobFileName(response);
                let bindaryData = [];
                bindaryData.push(response.body);
                let downloadLink = document.createElement('a');
                downloadLink.href = window.URL.createObjectURL(new Blob(bindaryData, { type: 'blob' }));
                downloadLink.setAttribute('download', fileName);
                document.body.appendChild(downloadLink);
                downloadLink.click();

                this.loading = false;
            }))
    }
}









