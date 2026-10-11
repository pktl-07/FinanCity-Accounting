import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme, SubSink } from '../utils';
import { BehaviorSubject, Observable } from 'rxjs';
import { State, DataResult } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators';
import { Component, Inject, OnDestroy } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({ template: '' })
export abstract class BaseApiService extends BehaviorSubject<any> implements OnDestroy {

    private _http: HttpClient;
  
    private _route: string;
    protected subsink = new SubSink();

    constructor(http: HttpClient,
        @Inject('') route: string) {

        super(null);

        this._http = http;
        this._route = route;
    }

    ngOnDestroy(): void {
        this.subsink.unsubscribe();
    }

    excuteLazyLoadDataArray(path: string, expr: string = null): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({ 'expression': expr }));
        return this._http.get(`${environment.auth.api}/${this._route}/${path}`,
            { headers: headers })
            .pipe(
                map(response => response)
            );
    }

    asyncFinds(state: State = { filter: { filters: [], logic: "and" } }) {

        this.subsink.sink = this.excuteLazyLoadDataArray('get', JSON.stringify(state.filter))
            .subscribe(x => super.next(x));
    }

    asyncFindsByQuery(query: string = null): void {

        this.subsink.sink = this.excuteLazyLoadDataArray('get', JSON.stringify(query))
            .subscribe(x => super.next(x));
    }

    getPager(state: State = {
        filter: {
            filters: [],
            logic: 'and'
        },
        skip: 0,
        take: 14
    }) {
        this.subsink.sink = this.excuteLazyLoadPager(state)
            .subscribe(x => super.next(x));
    }

    public excuteLazyLoadPager(state: State): Observable<DataResult> {

        let currentPage = (state.skip / state.take) + 1;
        let headers = new HttpHeaders(getHeaderScheme({ 'expression': JSON.stringify(state.filter) }));

        return this._http.get(`${environment.auth.api}/${this._route}/pager/${currentPage}/${state.take}`,
            { headers: headers })
            .pipe(
                map(response => (<DataResult>{
                    data: response['Data'],
                    total: response['Total']
                }))
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

    private excuteLazyLoadData(path: string, key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this._http.get(`${environment.auth.api}/${this._route}/${path}/${key}`,
            { headers: headers }).pipe(
                map(response => response),
            );
    }

    findByKey(key: string = null): Observable<any> {

        return this.get(`getByKey/${key}`);
    }

    default(): Observable<any> {
        return this.get(`getDefault`);
    }

    defaultByKey(key: string): Observable<any> {
        return this.get(`getDefault/${key}`);
    }

    protected get(path: string, header?: {}): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme(header));
        return this._http.get(`${environment.auth.api}/${this._route}/${path}`, { headers: headers })
    }

    save(data: {}): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.post(`${environment.auth.api}/${this._route}/save`, data, { headers: headers })
    }

    update(data: any): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this._route}/update`, data, { headers: headers })
    }

    post(data: any): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this._route}/post`, data, { headers: headers })
    }

    reverse(key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this._route}/reverse?syskey=${key}`, { headers: headers })
    }

    void(key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.put(`${environment.auth.api}/${this._route}/void?syskey=${key}`, { headers: headers })
    }

    delete(key: string): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this._http.delete(`${environment.auth.api}/${this._route}/delete?syskey=${key}`, { headers: headers })
    }
}







