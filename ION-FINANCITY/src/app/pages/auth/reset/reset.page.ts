import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { AlertController, LoadingController } from '@ionic/angular';
import { UserAccountService } from 'src/app/services/user-account.service';
import { HttpErrorResponse } from '@angular/common/http';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'app-reset',
  templateUrl: './reset.page.html'
})
export class ResetPage implements OnInit, OnDestroy {

  public resetFormGroup: FormGroup;
  private _subsink = new SubSink();

  constructor(
    private _alertController: AlertController,
    private _loadingController: LoadingController,
    private _userAccount: UserAccountService,
    private _fb: FormBuilder,
  ) { }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this.resetFormGroup = this._fb.group({
      'Email': new FormControl()
    });
    this.resetFormGroup.get('Email').setValidators([Validators.required, Validators.email]);
  }

  async onResetClick() {

    this.resetFormGroup.get('Email').markAsTouched();
    if (this.resetFormGroup.valid) {

      const alert = await this._alertController.create({
        header: 'Password Reset',
        message: `The password reset link will be send to your associated email address.`,
        buttons: ['OK']
      });

      const loading = await this._loadingController.create({
        message: 'Please wait...',
      });

      await loading.present();

      this._subsink.sink = this._userAccount.requestResetPassword({ Email: this.resetFormGroup.get('Email').value })
        .subscribe(async () => {
          await loading.dismiss();
          await alert.present();
        }, async (error: HttpErrorResponse) => {
          await loading.dismiss();
          alert.message = JSON.stringify(error.error);
          await alert.present();
        });
    }
  }
}
