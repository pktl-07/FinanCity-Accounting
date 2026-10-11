import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/Infrastructure/common-interface';
import { GlobalConfigurationSubject } from 'src/app/mk-config';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class UserAccountService extends BaseApiService {

    constructor(private http: HttpClient,
        public global: GlobalConfigurationSubject) {

        super(http, `UserAccount`);
    }

    public getUserClaims() {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/${this.route}/GetUserClaims`, { headers: headers })
    }

    public changePassword(data: any) {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.put(`${environment.auth.api}/${this.route}/ChangePassword`, data, { headers: headers })
    }

    public requestResetPassword(data: any) {

        let headers = new HttpHeaders(getHeaderScheme({ 'No-Auth': 'True' }));
        return this.http.post(`${environment.auth.api}/${this.route}/RequestResetPassword`, data, { headers: headers })
    }

    public resetPassword(data: any) {

        let headers = new HttpHeaders(getHeaderScheme({ 'No-Auth': 'True' }));
        return this.http.put(`${environment.auth.api}/${this.route}/ResetPassword`, data, { headers: headers })
    }

    public SignOut() {

        return new Promise((resolve) => {

            localStorage.removeItem('userToken');
            localStorage.removeItem('email');
            localStorage.removeItem('isAuthenticate');
            localStorage.removeItem('profileName');

            environment.auth.email = null;
            environment.auth.isAuthenticate = false;
            environment.auth.profileName = null;
            this.global.FinanCityConfig.next(environment);

            resolve(null)
        })
    }
}