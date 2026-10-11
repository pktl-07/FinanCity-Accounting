import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private _route: Router) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        if (req.headers.get('No-Auth') == 'True') {
            return next.handle(req.clone())
        }

        if (localStorage.getItem('userToken') != null) {

            const clonedreq = req.clone({
                headers: req.headers.set('Authorization', `Bearer ${localStorage.getItem('userToken')}`)
            });
            return next.handle(clonedreq)
                .pipe(catchError(err => {
                    if (err instanceof HttpErrorResponse) {
                        if (err.status === 401 || err.status === 403) {
                            this._route.navigateByUrl('\Login')
                        }
                    }
                    return throwError(err)
                }));
        }
    }
}