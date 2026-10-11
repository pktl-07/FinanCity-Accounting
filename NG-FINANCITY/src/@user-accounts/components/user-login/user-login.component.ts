import { Component, OnDestroy, OnInit, ViewChild, } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { UserService } from './user.service';
import { HttpErrorResponse } from '@angular/common/http';
import { UserAccountService } from 'src/@user-accounts/services/user-account.service';
import { finalize } from 'rxjs/operators';
import { ToastService } from 'src/@common/components/p-toast/p-toast.service';
import { PConfirmDialogComponent } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.component';
import { PErrorComponent } from 'src/@common/components/p-confirm-dialog/p-error-component';
import { PageLoaderService } from 'src/@common/components/p-page-loader/p-page-loader.service';
import { SocialAuthService, FacebookLoginProvider, GoogleLoginProvider } from "angularx-social-login";
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-user-login',
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.scss'],
  providers: [
    UserService,
    ToastService,
  ]
})
export class UserLoginComponent implements OnInit, OnDestroy {

  @ViewChild('confirmDialog', { static: true })
  public _confirmDialog: PConfirmDialogComponent;

  public loginFormGroup: FormGroup;
  public resetPasswordFormGroup: FormGroup;
  public errorMessage: string;
  private _subsink = new SubSink();

  constructor(public _router: Router,
    private _authService: SocialAuthService,
    public pageLoaderService: PageLoaderService,
    private _pToastService: ToastService,
    private _userAccountService: UserAccountService,
    private _userService: UserService,
    private _fb: FormBuilder) {
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  public toggleResetPswed() {

    $('#logreg-forms .form-signin').toggle() // display:block or none
    $('#logreg-forms .form-reset').toggle() // display:block or none
  }

  ngOnInit(): void {

    this.loginFormGroup = this._fb.group({
      'UserName': new FormControl(),
      'Password': new FormControl()
    });

    this.resetPasswordFormGroup = this._fb.group({
      'Email': new FormControl()
    })

    this._subsink.sink = this.loginFormGroup.get('UserName').valueChanges.subscribe((value) => {
      this.errorMessage = null;
    })

    this._subsink.sink = this.loginFormGroup.get('Password').valueChanges.subscribe((value) => {
      this.errorMessage = null;
    })

    this.loginFormGroup.get('UserName').setValidators([Validators.required]);
    this.loginFormGroup.get('Password').setValidators([Validators.required, Validators.minLength(6)]);
    this.resetPasswordFormGroup.get('Email').setValidators([Validators.required, Validators.email]);
  }

  public onSignInClick() {

    this.loginFormGroup.get('UserName').markAsTouched();
    this.loginFormGroup.get('Password').markAsTouched();

    if (this.loginFormGroup.valid) {

      this.pageLoaderService.loading = true;
      this._subsink.sink = this._userService.userPasswordAuthentication(this.loginFormGroup.get('UserName').value,
        this.loginFormGroup.get('Password').value)
        .pipe(
          finalize(() => this.pageLoaderService.loading = false))
        .subscribe(() => {
          this._router.navigate(['']);
        }, (error: HttpErrorResponse) => {
          this.presentError(error);
        });
    }
  }

  async socialSignIn(value: 'google' | 'facebook') {

    switch (value) {
      case 'google':
        this._authService.signIn(GoogleLoginProvider.PROVIDER_ID)
          .then((user) => {
            this.externalSignIn({ id: user.id, social: value, username: user.email })
          }).catch((error) => { this.presentError(error) });
        break;
      case 'facebook':
        this._authService.signIn(FacebookLoginProvider.PROVIDER_ID)
          .then((user) => {
            this.externalSignIn({ id: user.id, social: value, username: user.email })
          }).catch((error) => { })
        break;
    }
  }

  public onResetPasswordClick() {

    this.resetPasswordFormGroup.get('Email').markAsTouched();
    if (this.resetPasswordFormGroup.valid) {

      this._subsink.sink = this._userAccountService.requestResetPassword({ Email: this.resetPasswordFormGroup.get('Email').value })
        .subscribe(() => {
          this._pToastService.open({
            data: {
              message: `The request has been send, please check your e-mail.`,
              title: 'Information'
            }
          });
          this.errorMessage = null;
        }, (error) => {
          this.presentError(error)
        });
    }
  }

  private externalSignIn(param: { social: 'google' | 'facebook', username: string, id: string }) {

    this.pageLoaderService.loading = true;
    this._subsink.sink = this._userService.useExternalAuthentication({
      external: param.social,
      username: param.username,
      id: param.id
    }).pipe(
      finalize(() => this.pageLoaderService.loading = false))
      .subscribe(() => {
        this._router.navigate(['']);
      }, (error: HttpErrorResponse) => {
        this.presentError(error);
        this._authService.signOut();
      });
  }

  private presentError(error: HttpErrorResponse) {

    if (error?.error) {

      const d = (error.error.error_description as string).replace(/--(.*):/g, '*');
      const m = d.split('*');

      let a = this._confirmDialog.open({
        title: 'Errors',
        content: PErrorComponent,
        action: [{ text: 'OK' }],
      });

      a.content.instance.errors = m;
      this.errorMessage = m[0];
    } else {

      let a = this._confirmDialog.open({
        title: 'Errors',
        content: PErrorComponent,
        action: [{ text: 'OK' }],
      });
      a.content.instance.errors = ['Error please try again.'];
    }
  }
}
