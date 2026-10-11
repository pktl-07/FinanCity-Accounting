import { Component, OnInit, ViewChild, Inject, OnDestroy } from '@angular/core';
import { TaxService } from 'src/@accounts/services/tax.service';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { AccountSetupDetailComponent } from '../account-setup-detail/account-setup-detail.component';
import { catchError, finalize } from 'rxjs/operators';
import { taxType, taxCategory, makeFormTouched, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-tax-detail',
  templateUrl: './tax-detail.component.html',
  providers: [TaxService,
    { provide: 'outputTaxAccount', useClass: AccountSetupService },
    { provide: 'inputTaxAccount', useClass: AccountSetupService },
    PWindowComponentService]
})
export class TaxDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  _taxCategory = [];
  _taxType = [];

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _taxService: TaxService,
    public _windowService: PWindowComponentService,
    @Inject('inputTaxAccount') public _inputTaxAccountService: AccountSetupService,
    @Inject('outputTaxAccount') public _outputTaxAccountService: AccountSetupService) {

    super(fb);
  }

  ngOnInit() {

    this._taxType = taxType();
    this._taxCategory = taxCategory();

    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NewButton({ action: () => this._taxService.default() })
      .SaveButton({
        action: () => this._taxService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._taxService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._taxService.delete(this.form.value.SysKey)
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  private _accountCodefilterText: string;
  handelTaxAccountFilterChange(param: { value: string, service: BaseApiService }) {

    this._accountCodefilterText = param.value;
    if (param.value.length >= 3) {
      param.service.asyncFinds({
        filter: {
          logic: 'or',
          filters: kendoFilter([
            { field: 'AccCode', value: `<#${param.value}`, operator: 'contains', logic: 'or' },
            { field: 'AccDescription', value: `${param.value}#>`, operator: 'contains', logic: 'and' },
            { field: 'AccountType', value: 1, operator: 'eq', logic: 'and' }
          ])
        }
      });
    }
  }

  loadData(param: { SysKey?: string, TaxCode?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._taxService.findByKey(param.SysKey) : this._taxService.default();

      this._subsink.sink = subscribe
        .pipe(catchError(error => {
          reject(error);
          return error;
        }),
          finalize(() => { resolve(this.form) }))
        .subscribe(data => {

          if (data.InputTaxSK && data.InputTaxSK != null)
            this._inputTaxAccountService.asyncFinds({
              filter: {
                filters: kendoFilter([{ field: 'SysKey', value: data.InputTaxSK, operator: 'eq', logic: 'and', }]),
                logic: 'and'
              }
            });

          if (data.OutputTaxSK && data.OutputTaxSK != null)
            this._outputTaxAccountService.asyncFinds({
              filter: {
                filters: kendoFilter([{ field: 'SysKey', value: data.OutputTaxSK, operator: 'eq', logic: 'and', }]),
                logic: 'and'
              }
            });

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.TaxCode)
            this.form.get('TaxCode').patchValue(param.TaxCode);

          this.form.get('TaxCode').setValidators(Validators.required);
          this.form.get('Description').setValidators(Validators.required);
          this.form.get('InputTaxSK').setValidators(Validators.required);
          this.form.get('OutputTaxSK').setValidators(Validators.required);
        })
    })
  }

  addNewAccount(param: { field: string, service: BaseApiService }) {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Account Setup', componentType: AccountSetupDetailComponent, state: state });
    let windowComponent = window.content.instance as AccountSetupDetailComponent;
    windowComponent.loadData({}).then(f => {

      f.get('AccDescription').patchValue(this._accountCodefilterText);

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(item => {
          param.service.asyncFinds({
            filter: {
              filters: kendoFilter([{ field: 'SysKey', value: item.data.SysKey, operator: 'eq', logic: 'and' }]),
              logic: 'and'
            }
          });

          this.form.get(param.field).setValue(item.data.SysKey);
          this._windowService.close();
        });
    })
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
