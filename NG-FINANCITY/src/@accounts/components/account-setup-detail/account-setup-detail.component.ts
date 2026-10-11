import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { AccountCategoryService } from 'src/@accounts/services/account-category.service';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { catchError, finalize, tap } from 'rxjs/operators';
import { accountType, makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-account-setup-detail',
  templateUrl: './account-setup-detail.component.html',
  providers: [
    AccountSetupService,
    AccountCategoryService
  ]
})
export class AccountSetupDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  _accountType = [];
  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _accountSetupService: AccountSetupService,
    public _accountCategoryService: AccountCategoryService) {

    super(fb);
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };

  ngOnInit() {

    this._accountCategoryService.asyncFinds();
    this._accountType = accountType();
    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._accountSetupService.default().pipe(tap(() => {
          this.form.get('Classification').patchValue(null);
          this.form.get('PrefixAccountCode').patchValue(null);
          this.form.get('PostfixAccountCode').patchValue(null);
        })
        )
      })
      .SaveButton({
        action: () => this._accountSetupService.save(this.getFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._accountSetupService.update(this.getFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._accountSetupService.delete(this.form.value.SysKey)
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  getClisification(value: any) {
    switch (value) {
      case 1:
        return 'Asset';
      case 2:
        return 'Liability';
      case 3:
        return 'Equity';
      case 4:
        return 'Income';
      case 5:
        return 'Cost Of Sales';
      case 6:
        return 'Expense';
      case 8:
        return 'Other Income';
      case 9:
        return 'Other Expense';
    }
  }

  getDRCR(value: any) {

    switch (value) {
      case "1":
      case "6":
      case "9":
      case "5":
        return "DR";
      default:
        return "CR";
    }
  }

  loadData(param: { SysKey?: string, AccDescription?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._accountSetupService.findByKey(param.SysKey) : this._accountSetupService.default();
      this._subsink.sink = subscribe.pipe(
        catchError(error => {
          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          let prefix = null;
          let postfix = null;

          if (param.SysKey) {

            prefix = (data['AccCode'] as string).split('-')[0];
            postfix = (data['AccCode'] as string).split('-')[1];
          }

          if (param.AccDescription) {
            this.form.get('AccDescription').patchValue(param.AccDescription);
          }

          this.form.addControl('PrefixAccountCode', new FormControl(prefix));
          this.form.addControl('PostfixAccountCode', new FormControl(postfix == null ? null : parseInt(postfix)));
          this.form.addControl('Classification', new FormControl());

          this._subsink.sink = this.form.get('CategorySK').valueChanges
            .subscribe(d => {

              let p = (d as string).substring(0, 1);
              //assign prefix account code
              this.form.get('PrefixAccountCode').setValue(p);
              //assign DRCR type
              this.form.get('DRCR').setValue(this.getDRCR(p));
              //assign accouont classification type
              this.form.get('Classification').setValue(this.getClisification(parseInt(p)));
            });

          if (param.SysKey) {
            this.form.get('CategorySK').patchValue(this.form.get('CategorySK').value)
          }

          this.form.get('PostfixAccountCode').setValidators([Validators.required,
          Validators.minLength(4),
          Validators.maxLength(4)]);

          this.form.get('AccDescription').setValidators(Validators.required);
          this.form.get('CategorySK').setValidators(Validators.required);
          this.form.get('AccountType').setValidators(Validators.required);
        })
    })
  }

  isReadonly() {
    return this.form.get('RecordStatus').value > 0;
  }

  private getFormValue() {

    this.form.get('SysKey').patchValue(`${this.form.get('PrefixAccountCode').value}${this.form.get('PostfixAccountCode').value}`);
    this.form.get('AccCode').patchValue(`${this.form.get('PrefixAccountCode').value}-${this.form.get('PostfixAccountCode').value}`);

    return this.form.value;
  }
}
