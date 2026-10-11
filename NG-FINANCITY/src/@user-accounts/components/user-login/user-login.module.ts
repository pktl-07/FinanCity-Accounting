import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserLoginComponent } from './user-login.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PConfirmDialogModule } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.module';
import { RouterModule } from '@angular/router';

import {
  GoogleLoginProvider,
  FacebookLoginProvider,
  SocialAuthServiceConfig,
  SocialLoginModule
} from "angularx-social-login";

const google_client_id: string =
  "780792628353-milrmekg6l0kkj2fv60h6iq9bt8oi4dr.apps.googleusercontent.com";

const facebook_client_id: string =
  "680049369295899";

@NgModule({
  declarations: [UserLoginComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SocialLoginModule,
    FormsModule,
    PConfirmDialogModule,
    RouterModule.forChild([{ path: '', component: UserLoginComponent }])
  ],
  providers: [
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(google_client_id),
          },
          {
            id: FacebookLoginProvider.PROVIDER_ID,
            provider: new FacebookLoginProvider(facebook_client_id),
          },
        ],
      } as SocialAuthServiceConfig,
    }
  ],
})
export class UserLoginModule { }
