import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Storage } from '@ionic/storage';
import { Observable, from, throwError } from 'rxjs';
import { catchError, mergeMap } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    readonly tokenKey: string = "userToken";

    constructor(private _route: Router,
        private _nativeStorage: Storage) {
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return from(this._nativeStorage.get(this.tokenKey))
            .pipe(mergeMap((token: string) => {

                if (request.headers.get('No-Auth') == 'True') {
                    return next.handle(request.clone());
                }
                if (token && token != null) {
                    const clonedreq = request.clone({
                        headers: request.headers.set('Authorization', `Bearer ${token}`)
                    })
                    return next.handle(clonedreq).pipe(
                        catchError(error => {
                            if (error instanceof HttpErrorResponse) {
                                if (error.status === 401)
                                    this._route.navigate(['login'])
                            }
                            return throwError(error)
                        })
                    )
                }
                return next.handle(request);
            }))
    }
}