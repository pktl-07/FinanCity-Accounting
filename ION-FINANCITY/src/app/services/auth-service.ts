import { UserAccountService } from './user-account.service';
import { tap, mergeMap, map } from 'rxjs/operators';
import { Injectable, OnDestroy } from '@angular/core';
import { Storage } from '@ionic/storage';
import { SubSink } from 'src/@common/utils';
import { FileService } from 'src/@common/components/native-devices/fc-file-service';
import { GooglePlus } from '@ionic-native/google-plus/ngx';
import { Facebook } from '@ionic-native/facebook/ngx';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService implements OnDestroy {

    private _subsink = new SubSink();
    readonly tokenKey: string = "userToken";

    constructor(private _userAccount: UserAccountService,
        private _nativeStorage: Storage,
        private _fileService: FileService,
        private _googlePlus: GooglePlus,
        private _facebook: Facebook) {
    }

    public destory() {
        this._subsink.unsubscribe();
    }

    ngOnDestroy(): void {
        this.destory();
    }

    SignInWithPassword(email: string, password: string) {

        return this._userAccount.authenticatedByPassword(email, password)
            .pipe(
                tap((token: any) => {
                    environment.auth.loginType = 'password';
                    this.setToken(token);
                }));
    }

    SignInWithGoogle(email: string, id: string) {

        return this._userAccount.authenticatedByGoogle(email, id)
            .pipe(
                tap((token: any) => {
                    environment.auth.loginType = 'google';
                    this.setToken(token);
                }));
    }

    SignInWithFacebook(email: string, id: string) {

        return this._userAccount.authenticatedByFacebook(email, id)
            .pipe(
                tap((token: any) => {
                    environment.auth.loginType = 'facebook';
                    this.setToken(token);
                }));
    }

    SignInWithApple(email: string, id: string) {

        return this._userAccount.authenticatedByApple(email, id)
            .pipe(
                tap((token: any) => {
                    environment.auth.loginType = 'ios';
                    this.setToken(token);
                }));
    }

    Logout() {

        return new Promise((resolve) => {
            if (environment.auth.loginType == 'google') {
                this._googlePlus.logout().then(() => {
                    this._nativeStorage.remove(this.tokenKey).then(() => {
                        this.removeToken();
                        resolve(null);
                    })
                })
            }
            else if (environment.auth.loginType == 'facebook') {
                this._facebook.logout().then(() => {
                    this._nativeStorage.remove(this.tokenKey).then(() => {
                        this.removeToken();
                        resolve(null);
                    })
                })
            } else {
                this._nativeStorage.remove(this.tokenKey).then(() => {
                    this.removeToken();
                    resolve(null);
                })
            }
        })
    }

    private removeToken() {

        environment.general.profilePhoto.next(null);
        environment.auth.SPSK = null;
        environment.auth.email = null;
        environment.auth.isAuthenticate.next(false);
        environment.auth.licenseKey = null;
        environment.auth.profileName.next(null);
    }

    private setToken(token: any) {

        this._nativeStorage.set(this.tokenKey, token.access_token).then(() => {

            this._subsink.sink = this._userAccount.getUserClaims()
                .pipe(mergeMap((claim: any) => {
                    return this._fileService.url({ fileType: 1, uploadId: claim.SPSK })
                        .pipe(map((url: string) => {
                            environment.general.profilePhoto.next(url);
                            return claim;
                        }))
                }))
                .subscribe((claim: any) => {

                    environment.auth.profileName.next(`${claim.FirstName} ${claim.LastName}`);
                    environment.auth.SPSK = claim.SPSK;
                    environment.auth.email = claim.Email;
                    // environment.auth.api = claim.Api;
                    environment.auth.isAuthenticate.next(true);
                });
        })
    }
}