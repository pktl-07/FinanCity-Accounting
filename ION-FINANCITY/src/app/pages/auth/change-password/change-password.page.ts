import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { UserAccountService } from 'src/app/services/user-account.service';
import { FormUtilityService } from 'src/app/services/form-utility.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.page.html',
  providers: [UserAccountService,
    FormUtilityService]
})
export class ChangePasswordPage implements OnInit {

  public form: FormGroup;

  constructor(private _fb: FormBuilder,
    private _formUtilityService: FormUtilityService,
    private _userAccountService: UserAccountService,
  ) { }

  ngOnInit() {

    this.form = this._fb.group({
      'CurrentPassword': new FormControl(),
      'NewPassword': new FormControl(),
      'ConfirmPassword': new FormControl(),
    });

    this.form.get('CurrentPassword').setValidators([Validators.required, Validators.minLength(6)]);
    this.form.get('NewPassword').setValidators([Validators.required, Validators.minLength(6)]);
    this.form.get('ConfirmPassword').setValidators([Validators.required, Validators.minLength(6)]);
  }

  async handleOk() {

    this.form.markAllAsTouched();

    if (this.form.valid) {

      this._formUtilityService.action(this._userAccountService.changePassword({
        CurrentPassword: this.form.get('CurrentPassword').value,
        NewPassword: this.form.get('NewPassword').value,
        ConfirmPassword: this.form.get('ConfirmPassword').value
      }), 'change password');
    }
  }
}
