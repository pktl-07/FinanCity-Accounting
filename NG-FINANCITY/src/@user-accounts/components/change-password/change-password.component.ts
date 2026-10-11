import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { UserAccountService } from 'src/@user-accounts/services/user-account.service';
import { ToastService } from 'src/@common/components/p-toast/p-toast.service';
import { PConfirmDialogComponent } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.component';
import { PErrorComponent } from 'src/@common/components/p-confirm-dialog/p-error-component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
  providers: [
    UserAccountService,
    ToastService
  ]
})
export class ChangePasswordComponent implements OnInit , OnDestroy {

  public formGroup: FormGroup;

  @ViewChild('confirmDialog', { static: true })
  public _confirmDialog: PConfirmDialogComponent;
  private _subsink = new SubSink();
  constructor(public _router: Router,
    private _pToastService: ToastService,
    private _userAccountService: UserAccountService,
    private _fb: FormBuilder) {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit(): void {

    this.formGroup = this._fb.group({
      'CurrentPassword': new FormControl(),
      'NewPassword': new FormControl(),
      'ConfirmPassword': new FormControl(),
    });

    this.formGroup.get('CurrentPassword').setValidators([Validators.required]);
    this.formGroup.get('NewPassword').setValidators([Validators.required, Validators.min(6)]);
    this.formGroup.get('ConfirmPassword').setValidators([Validators.required, Validators.min(6)]);
  }

  public onChangePasswordClick() {

    this.formGroup.get('CurrentPassword').markAsTouched();
    this.formGroup.get('NewPassword').markAsTouched();
    this.formGroup.get('ConfirmPassword').markAsTouched();

    if (this.formGroup.valid == true) {

     this._subsink.sink= this._userAccountService.changePassword({
        CurrentPassword: this.formGroup.get('CurrentPassword').value,
        NewPassword: this.formGroup.get('NewPassword').value,
        ConfirmPassword: this.formGroup.get('ConfirmPassword').value
      })
        .subscribe((d: any) => {

          this._pToastService.open({
            data: {
              message: `Your password has been changed.`,
              title: 'Information'
            }
          });
        }, (error) => {

          let d = (error.error.Message as string).replace(/--(.*):/g, '*');
          let m = d.split('*');

          let a = this._confirmDialog.open({
            title: 'Errors',
            content: PErrorComponent,
            action: [{ text: 'OK' }],
          });

          a.content.instance.errors = m;
        })
    }
  }
}
