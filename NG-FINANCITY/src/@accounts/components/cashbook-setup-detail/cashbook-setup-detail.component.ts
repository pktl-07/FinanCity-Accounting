import { Component, OnInit, ViewChild, Inject, OnDestroy } from '@angular/core';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { Observable } from 'rxjs';
import { finalize, catchError } from 'rxjs/operators';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { CurrencyRateDetailComponent } from 'src/@sales/components/currency-rate-detail/currency-rate-detail.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { AccountSetupDetailComponent } from '../account-setup-detail/account-setup-detail.component';
import { makeFormTouched, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-cashbook-setup-detail',
  templateUrl: './cashbook-setup-detail.component.html',
  providers: [AccountSetupService, CashbookSetupService,
    { provide: 'account-setup-1', useClass: AccountSetupService },
    { provide: 'account-setup-2', useClass: AccountSetupService },
    { provide: 'account-setup-3', useClass: AccountSetupService },
    CurrencyRateService,
    PWindowComponentService]
})
export class CashbookSetupDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  _cashBookType = [{ value: 1, text: 'CASHBOOK' },
  { value: 2, text: 'BANK' }];

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _windowService: PWindowComponentService,
    public _currencyRateService: CurrencyRateService,
    @Inject('account-setup-1') public _accountService1: AccountSetupService,
    @Inject('account-setup-2') public _accountService2: AccountSetupService,
    @Inject('account-setup-3') public _accountService3: AccountSetupService,
    public _cashBookSetupService: CashbookSetupService) {

    super(fb);
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };

  ngOnInit() {

    this._currencyRateService.asyncFinds();

    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NewButton({ action: () => this._cashBookSetupService.default() })
      .SaveButton({
        action: () => this._cashBookSetupService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .UpdateButton({
        action: () => this._cashBookSetupService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .DeleteButton({
        action: () => this._cashBookSetupService.delete(this.form.value.SysKey),
      })
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  loadData(param: { SysKey?: string, CashbookName?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe: Observable<any>;

      if (param.SysKey)
        subscribe = this._cashBookSetupService.findByKey(param.SysKey);
      else
        subscribe = this._cashBookSetupService.default();

      this._subsink.sink = subscribe.pipe(
        catchError(error => {
          reject(error);
          return error;
        }),
        finalize(() => { resolve(this.form) }))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.CashbookName)
            this.form.get('CashbookName').patchValue(param.CashbookName);

          if (data.AccSK != null) {
            this._accountService1.asyncFinds({
              filter: {
                filters: kendoFilter([{ field: 'SysKey', value: data.AccSK, operator: 'eq', logic: 'and' }]),
                logic: 'and'
              }
            });
          }

          if (data.ForeignExchangeGainSK != null) {
            this._accountService2.asyncFinds({
              filter: {
                filters: kendoFilter([{ field: 'SysKey', value: data.ForeignExchangeGainSK, operator: 'eq', logic: 'and' }
                ]),
                logic: 'and'
              }
            });
          }

          if (data.ForeignExchangeLossSK != null) {
            this._accountService3.asyncFinds({
              filter: {
                filters: kendoFilter([{ field: 'SysKey', value: data.ForeignExchangeLossSK, operator: 'eq', logic: 'and', }
                ]),
                logic: 'and'
              }
            });
          }

          this._subsink.sink = this.form.get('CBType').valueChanges
            .subscribe((d: any) => {
              if (d == 1) {

                this.form.get('BankAccountNo').setValue(null);
                this.form.get('BankAccountNo').disable();
              }
              else
                this.form.get('BankAccountNo').enable();
            });

          this.form.get('CashbookName').setValidators([Validators.required]);
          this.form.get('Description').setValidators([Validators.required]);
          this.form.get('AccSK').setValidators([Validators.required]);
          this.form.get('ForeignExchangeGainSK').setValidators([Validators.required]);
          this.form.get('ForeignExchangeLossSK').setValidators([Validators.required]);
        })
    });
  }

  private _accountCodeFilterText: string;
  handelAccountFilterChange(param: { value: string, service: BaseApiService }) {

    this._accountCodeFilterText = param.value;
    if (param.value.length >= 3) {
      param.service.asyncFinds({
        filter: {
          logic: 'or',
          filters: this.accountDropDownSetting(param.value),
        }
      });
    }
  }

  accountDropDownSetting(value: string) {

    return kendoFilter([
      { field: 'AccCode', value: `<# ${value}`, operator: 'contains', logic: 'or' },
      { field: 'AccDescription', value: `${value} #>`, operator: 'contains', logic: 'and' },
      { field: 'AccountType', value: 1, operator: 'eq', logic: 'and' },
    ])
  }

  private _currencyFilterText: string;

  addNewCurrency() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Currency', componentType: CurrencyRateDetailComponent, state: state });
    let windowComponent = window.content.instance as CurrencyRateDetailComponent;
    windowComponent.loadData({ CurrencyName: this._currencyFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(data => {

        this._currencyRateService.asyncFinds();
        this._windowService.close();
      });
  }

  addNewAccount(param: { field: string, service: BaseApiService }) {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Account Setup', componentType: AccountSetupDetailComponent, state: state });
    let windowComponent = window.content.instance as AccountSetupDetailComponent;
    windowComponent.loadData({ AccDescription: this._accountCodeFilterText });

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
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
