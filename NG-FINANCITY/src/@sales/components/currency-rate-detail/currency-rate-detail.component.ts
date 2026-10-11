import { Component, OnInit, ViewChild, Inject, OnDestroy } from '@angular/core';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { catchError, finalize } from 'rxjs/operators';
import { makeFormTouched, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-currency-rate-detail',
  templateUrl: './currency-rate-detail.component.html',
  styles: [
    `
      .red {
        color : var(--danger);
      }
    `
  ],
  providers: [
    CurrencyRateService,
    CurrencyService,
  ]
})
export class CurrencyRateDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  operator = ['*', '/'];
  currCode: string;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _currencyRateService: CurrencyRateService,
    public _currencyService: CurrencyService) {

    super(fb);

    _currencyService.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NewButton({ action: () => this._currencyRateService.default() })
      .SaveButton({
        action: () => this._currencyRateService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .UpdateButton({
        action: () => this._currencyRateService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
  }

  onChangeCurrencyCode(value: string) {

    this.form.get('Rate').setValue(0);
    this.form.get('CompareRate').setValue(0);
    this.form.get('BaseRate').setValue(0);

    this._subsink.sink = this._currencyService
      .findByKey(value)
      .subscribe(d => {
        this.currCode = d.Currency_AlphaCode;
      });
  }

  handelCurrencyCodeFilterChange(value: string) {

    this._currencyService.asyncFinds({
      filter: {
        logic: 'or',
        filters: this.currencyDropDownSetting(value)
      }
    });
  }

  currencyDropDownSetting(value: string) {

    return kendoFilter([
      { field: 'Currency_Name', value: value, operator: 'contains', logic: 'or' },
      { field: 'Currency_AlphaCode', value: value, operator: 'contains', logic: 'or' },
      { field: 'Country', value: value, operator: 'contains', logic: 'or' }
    ])
  }

  loadData(param: { SysKey?: string, CurrencyName?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._currencyRateService.findByKey(param.SysKey) : this._currencyRateService.default();

      this._subsink.sink = subscribe.pipe(
        catchError(error => {
          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.CurrencyName)
            this.form.get('Description').patchValue(param.CurrencyName)

          this.form.addControl('CompareRate', new FormControl());
          this.form.addControl('BaseRate', new FormControl());

          this.calculateRate();

          this.form.get('SysKey').setValidators([Validators.required]);
          this.form.get('Rate').setValidators([Validators.required, Validators.min(0.0000000000000000000000000009)]);
          this.form.get('Description').setValidators([Validators.required]);
          this.form.get('Symbol').setValidators([Validators.required]);

          if (data.SysKey != null) { // set curr code

            this._subsink.sink = this._currencyService.findByKey(data.SysKey)
              .subscribe(d => {
                this.currCode = d.Currency_AlphaCode;
              });
          }

          if (param.SysKey && param.SysKey != null)
            this._currencyService.asyncFinds({
              filter: {
                filters: kendoFilter([
                  { field: 'SysKey', value: param.SysKey, operator: 'eq', logic: 'and', }
                ]),
                logic: 'and'
              }
            });
        })
    })
  }

  onChangeRate() {

    this.calculateRate();
  }

  onChangeOperator() {

    this.calculateRate();
  }

  calculateRate() {

    let op = this.form.get('Operator').value;
    let rate = this.form.get('Rate').value;

    switch (op) {

      case '*':

        this.form.get('CompareRate').setValue(rate / 1);
        if (rate != 0)
          this.form.get('BaseRate').setValue(1 / rate);
        else
          this.form.get('BaseRate').setValue(0);
        break;

      case '/':

        this.form.get('CompareRate').setValue(1 / rate);
        if (rate != 0)
          this.form.get('BaseRate').setValue(rate / 1);
        else
          this.form.get('BaseRate').setValue(0);
        break;
    }
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
