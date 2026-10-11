import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth-service';
import { LoadingController, Platform, ToastController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject } from 'rxjs';
import { AppVersion } from '@ionic-native/app-version/ngx';
import { IFCConfig, SubSink } from 'src/@common/utils';
import { GooglePlus } from '@ionic-native/google-plus/ngx';
import { Facebook } from '@ionic-native/facebook/ngx';
import { SignInWithApple, AppleSignInResponse, ASAuthorizationAppleIDRequest } from '@ionic-native/sign-in-with-apple/ngx';
import { SQLiteService } from 'src/app/services/sqlite.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styles: [
    `
    .login-panel{
      position: absolute;
      height:auto;
      width:100%;
      display:flex;
      margin:auto;
      align-items: center;
      justify-content: center;
    }
    .decorate-panel{
      background-color:rebeccapurple;
      height:110px;
    }
    `,
  ],
  providers: [AppVersion,
    GooglePlus,
    Facebook,
    SignInWithApple,
    ASAuthorizationAppleIDRequest,
  ],
})
export class LoginPage implements OnInit, OnDestroy {

  private _subsink = new SubSink();
  public loginFormGroup: FormGroup;
  public versionNumberSubject = new Subject();

  config : IFCConfig = environment;

  constructor(
    private _authService: AuthService,
    private _appVersion: AppVersion,
    private _googlePlus: GooglePlus,
    private _facebook: Facebook,
    private _sqlLiteService: SQLiteService,
    private _loadingController: LoadingController,
    private _fb: FormBuilder,
    private _toastController: ToastController,
    public _platform: Platform,
    private _signInWithApple: SignInWithApple) { }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  ngOnInit() {

    if (this._platform.is('cordova')) {

      this._appVersion.getVersionNumber()
        .then(version => {
          this.versionNumberSubject.next(version);
        });
    }

    this.loginFormGroup = this._fb.group({
      'UserName': new FormControl(),
      'Password': new FormControl()
    });

    this.loginFormGroup.get('UserName').setValidators([Validators.required]);
    this.loginFormGroup.get('Password').setValidators([Validators.required, Validators.minLength(6)]);
  }

  async handlePasswordLogin() {

    this.loginFormGroup.get('UserName').markAsTouched();
    this.loginFormGroup.get('Password').markAsTouched();

    const loading = await this._loadingController.create({
      message: 'Please wait...',
    });

    await loading.present();

    if (this.loginFormGroup.valid) {
      this._subsink.sink = this._authService.SignInWithPassword(this.loginFormGroup.get('UserName').value,
        this.loginFormGroup.get('Password').value)
        .subscribe(async () => {
          await loading.dismiss();
        }, async (error: HttpErrorResponse) => {
          await this.presentToast(error.error.error_description);
          await loading.dismiss();
        });
    }
  }

  async handleGoogleLogin() {

    let googleKey = this._platform.is('android') == true ?
      '780792628353-milrmekg6l0kkj2fv60h6iq9bt8oi4dr.apps.googleusercontent.com' ://android api key
      '780792628353-4ljpp203l96lbuto6buaq0n1fvbmrcns.apps.googleusercontent.com';//ios api key 

    this._googlePlus.login({
      'webClientId': googleKey,
      'scopes': 'email',
      'offline': true,
    }).then(async (result) => {

      const loading = await this._loadingController.create({
        message: 'Please wait...',
      });
      this._subsink.sink = this._authService.SignInWithGoogle(result.email, result.id)
        .subscribe(async () => {
          await loading.dismiss();
        }, async (error: HttpErrorResponse) => {
          await this.presentToast(error.error.error_description);
          await loading.dismiss();
        });

    }).catch(() => this.presentToast(`Login fail with error. Please try again.`))
  }

  async handleFacebookLogin() {

    this._facebook.login(['public_profile', 'email'])
      .then(async () => {

        const profile = await this._facebook.api('me?fields=id,name,email,first_name', []);
        const loading = await this._loadingController.create({ message: 'Please wait...' });

        this._subsink.sink = this._authService.SignInWithFacebook(profile['email'], profile['id'])
          .subscribe(async () => await loading.dismiss(),
            async (error) => {
              await this.presentToast(error.error.error_description);
              await loading.dismiss();
            })
      }).catch(() => this.presentToast(`Login fail with error. Please try again.`))
  }

  async handleAppleLogin() {

    this._signInWithApple.signin({
      requestedScopes: [
        ASAuthorizationAppleIDRequest.ASAuthorizationScopeFullName,
        ASAuthorizationAppleIDRequest.ASAuthorizationScopeEmail
      ]
    }).then(async (res: AppleSignInResponse) => {

      const loading = await this._loadingController.create({ message: 'Please wait...' });
      this._sqlLiteService.keepCreditial(res.email).then((email: string) => {
        this._subsink.sink = this._authService.SignInWithApple(email, res.identityToken)
          .subscribe(async () => {
            await loading.dismiss()
          },
            async (error) => {
              await this.presentToast(error.error.error_description);
              await loading.dismiss();
            })
      })
    }).catch((err) => {
      this.presentToast(`Login fail with error. Please try again.`)
    });
  }

  private async presentToast(message: string) {

    const toast = await this._toastController.create({
      message: message,
      duration: 5000
    })
    await toast.present();
  }
}