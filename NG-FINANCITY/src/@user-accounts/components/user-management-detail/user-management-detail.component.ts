import { Component, OnInit, ViewChild } from '@angular/core';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { catchError, finalize } from 'rxjs/operators';
import { UserAccountService } from 'src/@user-accounts/services/user-account.service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { makeFormTouched, kendoFilter, createJsonFormGroup } from 'src/@common/service/utility-service';
import { InternationalNameSpliterService } from 'src/@common/service/internation.name.splitter.service';

@Component({
  selector: 'app-user-management-detail',
  templateUrl: './user-management-detail.component.html',
  providers: [
    UserAccountService,
    SalespersonManagementService,
    InternationalNameSpliterService
  ]
})
export class UserManagementDetailComponent extends DetailFormBase implements OnInit {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  constructor(fb: FormBuilder,
    public _nameConventionService: InternationalNameSpliterService,
    public _salespersonService: SalespersonManagementService,
    private _userAccountService: UserAccountService) {

    super(fb);
  }

  ngOnInit() {

    this._salespersonService.asyncFinds();

    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NewButton({ action: () => this._userAccountService.default() })
      .SaveButton({
        action: () => this._userAccountService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .UpdateButton({
        action: () => this._userAccountService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .DeleteButton({
        action: () => this._userAccountService.delete(this.form.value.SysKey)
      })
  }

  loadData(key?: string): Promise<FormGroup> {

    return new Promise<any>((resolve, reject) => {

      let subscribe = key ? this._userAccountService.findByKey(key) : this._userAccountService.default();

      subscribe.pipe(
        catchError(error => {

          reject(error);
          return error;
        }),
        finalize(() => {
          resolve(this.form)
        })).subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          this.form.get('PasswordLoginUserName').setValidators([Validators.required]);
          this.form.get('FirstName').setValidators([Validators.required]);
          this.form.get('LastName').setValidators([Validators.required]);
          this.form.get('Google').setValidators([Validators.required, Validators.email]);
          this.form.get('Facebook').setValidators([Validators.email]);
        })
    });
  }

  handelSalespersonFilterChange(param: string) {

    if (param.length >= 3) {

      this._salespersonService.asyncFinds({
        filter: {
          filters: kendoFilter([{
            field: 'Name', logic: 'and', operator: 'contains', value: param
          }]),
          logic: 'or'
        }
      });
    }
    if (param.length == 0) {
      this._salespersonService.asyncFinds();
    }
  }

  handleSalespersonChange(e: any) {

    this._salespersonService.findByKey(e)
      .subscribe(() => {

        let name: string = e['Name'];

        this.form.get('PasswordLoginUserName').setValue(name);
        this.form.get('FirstName').setValue(name.split(' ')[0])
        this.form.get('LastName').setValue(name.split(' ')[1]);

        this._nameConventionService.register({
          fristName: this.form.get('FirstName'),
          lastName: this.form.get('LastName'),
          sourceName: new FormControl(name),
          isLastNameFirst: new FormControl(true)
        })

        this._nameConventionService.watch();
      });
  }
}
