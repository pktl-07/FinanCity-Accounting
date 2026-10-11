import { Directive, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ISearchable, IWriteable, getHeaderScheme } from './common-interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { State } from '@progress/kendo-data-query';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { SubSink } from '../service/utility-service';
import { environment } from 'src/environments/environment';

@Directive()
export abstract class BaseApiService extends BehaviorSubject<any> implements IWriteable, ISearchable, OnDestroy {

    public loading: boolean;
    protected _http: HttpClient;
    protected subsink = new SubSink();

    constructor(http: HttpClient,
        public route: string) {

        super(null);
        this._http = http;
    }

    ngOnDestroy(): void {
        this.subsink.unsubscribe();
    }

    public excuteLazyLoadDataArray(path: string, expr: string = null): Observable<any> {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': expr }));

        return this._http.get(`${environment.auth.api}/${this.route}/${path}`,
            { headers: headers }).pipe(
                map(response => response),
                tap(() => this.loading = false)
            );
    }

    asyncFinds(state: State = { filter: { filters: [], logic: "and" } }): void {

        this.subsink.sink = this.excuteLazyLoadDataArray('get', JSON.stringify(state.filter))
            .subscribe(x => super.next(x));
    }

    asyncFindsByQuery(query: string = null): void {

        this.subsink.sink = this.excuteLazyLoadDataArray('get', JSON.stringify(query))
            .subscribe(x => super.next(x));
    }


    getPager(state: State = null): void {
        this.subsink.sink = this.excuteLazyLoadPager(state)
            .subscribe(x => super.next(x));
    }

    pager(state: State): Observable<any> {
        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;

        return this._http.get(`${environment.auth.api}/${this.route}/pager/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<GridDataResult>{
                    data: response['Data'],
                    total: response['Total']
                })),
                tap(() => this.loading = false)
            );
    }

    public excuteLazyLoadPager(state: State): Observable<GridDataResult> {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));
        let currentPage = (state.skip / state.take) + 1;
     
        return this._http.get(`${environment.auth.api}/${this.route}/pager/${currentPage}/${state.take}`,
            { headers: headers }).pipe(
                map(response => (<GridDataResult>{
                    data: response['Data'],
                    total: response['Total']
                })),
                tap(() => this.loading = false)
            );
    }

    finds(state: State = { filter: { filters: [], logic: "and" } }): Observable<any[]> {

        return this.get(`get`, {
            'expression': JSON.stringify(state.filter)
        });
    }

    asyncFindByKey(key: string = null): Observable<any> {

        return this.excuteLazyLoadData('getByKey', key);
    }

    excuteLazyLoadData(path: string, key: string): Observable<any> {

        this.loading = true;
        let headers = new HttpHeaders(getHeaderScheme());

        return this._http.get(`${environment.auth.api}/${this.route}/${path}/${key}`,
            { headers: headers }).pipe(
                map(response => response),
                tap(() => this.loading = false)
            );
    }

    findByKey(key: string = null): Observable<any> {

        const a = key == "null" || key == null ? this.default()
            : this.get(`getByKey/${key}`);
        return a;
    }

    default(): Observable<any> {

        return this.get(`getDefault`);
    }

    defaultByKey(key: string): Observable<any> {

        return this.get(`getDefault/${key}`);
    }

    protected get(path: string, header?: {}): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme(header));
        return this._http.get(`${environment.auth.api}/${this.route}/${path}`, { headers: headers });
    }

    save(data: {}): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.post(`${environment.auth.api}/${this.route}/save`, data, { headers: headers });
    }

    update(data: any): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this.route}/update`, data, { headers: headers });
    }

    post(data: any): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this.route}/post`, data, { headers: headers });
    }

    reverse(key: string): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this.route}/reverse?syskey=${key}`, { headers: headers });
    }

    void(key: string): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this.route}/void?syskey=${key}`, { headers: headers });
    }

    delete(key: string): Observable<any> {

        const headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.delete(`${environment.auth.api}/${this.route}/delete?syskey=${key}`, { headers: headers });
    }
}









