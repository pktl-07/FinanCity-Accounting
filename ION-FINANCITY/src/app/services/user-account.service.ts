import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { getHeaderScheme } from 'src/@common/utils';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class UserAccountService {

    readonly route: string = 'UserAccount';
    constructor(private http: HttpClient) {
    }

    public authenticatedByPassword(userName: string, password: string): Observable<any> {

        const data = `username=${userName}&password=${password}&domainUrl=${environment.auth.api}&grant_type=password&login_mode=salesperson`;
        const reqHeader = new HttpHeaders({ 'Content-Type': 'application/x-www-urlencoded', 'No-Auth': 'True' });
        return this.http.post(`${environment.auth.api}/token`, data, { headers: reqHeader })
    }

    public authenticatedByGoogle(email: string, id: string): Observable<any> {

        const data = `username=${email}&password=${id}&domainUrl=${environment.auth.api}&grant_type=password&login_mode=salesperson&external=google`;
        const reqHeader = new HttpHeaders({ 'Content-Type': 'application/x-www-urlencoded', 'No-Auth': 'True' });
        return this.http.post(`${environment.auth.api}/token`, data, { headers: reqHeader })
    }

    public authenticatedByFacebook(email: string, id: string): Observable<any> {

        const data = `username=${email}&password=${id}&domainUrl=${environment.auth.api}&grant_type=password&login_mode=salesperson&external=facebook`;
        const reqHeader = new HttpHeaders({ 'Content-Type': 'application/x-www-urlencoded', 'No-Auth': 'True' });
        return this.http.post(`${environment.auth.api}/token`, data, { headers: reqHeader })
    }

    public authenticatedByApple(email: string, id: string): Observable<any> {

        const data = `username=${email}&password=${id}&domainUrl=${environment.auth.api}&grant_type=password&login_mode=salesperson&external=ios`;
        const reqHeader = new HttpHeaders({ 'Content-Type': 'application/x-www-urlencoded', 'No-Auth': 'True' });
        return this.http.post(`${environment.auth.api}/token`, data, { headers: reqHeader })
    }

    public getUserClaims(): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme());
        return this.http.get(`${environment.auth.api}/${this.route}/GetUserClaims`, { headers: headers })
    }

    public changePassword(data: any): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({}));
        return this.http.put(`${environment.auth.api}/${this.route}/ChangePassword`, data, { headers: headers })
    }

    public requestResetPassword(data: any): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({ 'No-Auth': 'True' }));
        return this.http.post(`${environment.auth.api}/${this.route}/RequestResetPassword`, data, { headers: headers })
    }

    public resetPassword(data: any): Observable<any> {

        let headers = new HttpHeaders(getHeaderScheme({ 'No-Auth': 'True' }));
        return this.http.put(`${environment.auth.api}/${this.route}/ResetPassword`, data, { headers: headers })
    }
}