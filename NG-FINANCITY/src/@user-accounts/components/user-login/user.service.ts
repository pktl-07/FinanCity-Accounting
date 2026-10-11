import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { mergeMap, tap } from 'rxjs/operators';
import { UserAccountService } from 'src/@user-accounts/services/user-account.service';
import { GlobalConfigurationSubject } from 'src/app/mk-config';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class UserService {

    constructor(private http: HttpClient,
        private _userAccountService: UserAccountService,
        private _global: GlobalConfigurationSubject) {
    }

    useExternalAuthentication(param: { external: 'google' | 'facebook' | 'ios', username: string, id: string }): Observable<any> {

        const data = `username=${param.username}&password=${param.id}&grant_type=password&login_mode=admin&external=${param.external}`;
        return this.signIn(data)
    }

    userPasswordAuthentication(userName: string, password: string): Observable<any> {

        const data = `username=${userName}&password=${password}&grant_type=password&login_mode=admin`;
        return this.signIn(data)
    }

    private signIn(data: any): Observable<any> {

        const reqHeader = new HttpHeaders({ 'Content-Type': 'application/x-www-urlencoded', 'No-Auth': 'True' });
        this._userAccountService.SignOut();
        return this.http.post(`${environment.auth.api}/token`, data, { headers: reqHeader })
            .pipe(
                tap((response: any) => {

                    localStorage.setItem('userToken', response.access_token)
                }),
                mergeMap(() => {
                    return this._userAccountService.getUserClaims()
                        .pipe(
                            tap((response: any) => {

                                localStorage.setItem('isAuthenticate', 'true');
                                localStorage.setItem('email', response.Email);
                                localStorage.setItem('profileName', `${response.FirstName} ${response.LastName}`);

                                environment.auth.api = response.Api;
                                environment.auth.isAuthenticate = localStorage.getItem('isAuthenticate') == 'true' ? true : false;
                                environment.auth.email = localStorage.getItem('email');
                                environment.auth.profileName = localStorage.getItem('profileName');

                                this._global.FinanCityConfig.next(environment)
                            })
                        )
                }));
    }
}
