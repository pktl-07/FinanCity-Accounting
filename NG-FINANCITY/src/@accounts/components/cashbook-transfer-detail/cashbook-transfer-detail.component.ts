import { Component, OnInit, OnDestroy, Inject, ViewChild } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { catchError, finalize, map, mergeMap } from 'rxjs/operators';
import { createJsonFormGroup, makeFormTouched, SubSink } from 'src/@common/service/utility-service';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { CashbookTransferService } from 'src/@accounts/services/cashbook-transfer.service';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { RecapDailogComponent } from 'src/@common/components/p-recap-dialog/p-recap-dialog.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-cashbook-transfer',
  templateUrl: './cashbook-transfer-detail.component.html',
  providers: [
    PWindowComponentService,
    CashbookTransferService,
    CurrencyRateService,
    CurrencyService,
    { provide: 'to-cashbook', useClass: CashbookSetupService },
    { provide: 'from-cashbook', useClass: CashbookSetupService },
  ]
})
export class CashbookTransferDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild('recapDailog', { static: true }) private _recap: RecapDailogComponent;

  get readonly() {
    return this.form.get('RecordStatus').value >= 6;
  }

  private _subsink = new SubSink();

  public baseCurr = environment.general.currSK;

  constructor(fb: FormBuilder,
    public _windowService: PWindowComponentService,
    private _cashbookTranserferService: CashbookTransferService,
    private _currencyRateService: CurrencyRateService,
    private _currencyService: CurrencyService,
    @Inject('to-cashbook') public _toCashBookSetupService: CashbookSetupService,
    @Inject('from-cashbook') public _fromCashBookSetupService: CashbookSetupService) {

    super(fb);

    this._subsink.sink = this.OnSuccess
      .subscribe(d => {
        if (d.data) {

          let f_transaction = d.data.FromTransactionAmount;
          d.data.ToTransactionAmount = this.currencyChange(f_transaction, 'from');

          let t_transaction = d.data.ToTransactionAmount;
          d.data.FromTransactionAmount = this.currencyChange(t_transaction, 'to');
        }
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit(): void {

    this._fromCashBookSetupService.asyncFinds();
    this._toCashBookSetupService.asyncFinds();

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._cashbookTranserferService.default()
      })
      .SaveButton({
        action: () => this._cashbookTranserferService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._cashbookTranserferService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .PostButton({
        action: () => this._cashbookTranserferService.post(this.form.value),
        validate: () => {
          makeFormTouched(this.form); return this.form.valid;
        }
      })
      .ReverseButton({
        action: () => this._cashbookTranserferService.reverse(this.form.value.SysKey),
        validate: () => {
          makeFormTouched(this.form); return this.form.valid;
        }
      })
      .VoidButton({
        action: () => this._cashbookTranserferService.void(this.form.value.SysKey),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      }).RecapButton({
        action: () =>
          this._recap.open(this._cashbookTranserferService.recap(this.form.value.SysKey))
      });
  }

  loadData(param: { SysKey?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe: Observable<any>;

      if (param.SysKey)
        subscribe = this._cashbookTranserferService.findByKey(param.SysKey);
      else
        subscribe = this._cashbookTranserferService.default();

      this._subsink.sink = subscribe
        .pipe(catchError(error => {
          reject(error);
          return error;
        }),
          finalize(() => { resolve(this.form) }))
        .subscribe(data => {

          data['FromCurrCode'] = '';
          data['ToCurrCode'] = '';

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          this._subsink.sink = this.form.get('FromCBSysKey').valueChanges
            .subscribe(data => {

              this._subsink.sink = this.findCurrency(data).subscribe(c => {
                this.form.get('FromCurrCode').patchValue(c['CURRCODE'], { emitEvent: false });
                this.form.get('FROMCURRATE').patchValue(c['Rate'], { emitEvent: false });
                this.form.get('FROMCURROP').patchValue(c['CURROP'], { emitEvent: false });
                this.form.get('FROMCURRSK').patchValue(c['CURRSK'], { emitEvent: false });

                let d = this.form.get('FromTransactionAmount').value;
                this.form.get('ToTransactionAmount').patchValue(this.currencyChange(d, 'from'), { emitEvent: false });
              });
            });

          this._subsink.sink = this.form.get('ToCBSysKey').valueChanges
            .subscribe(data => {

              this._subsink.sink = this.findCurrency(data)
                .subscribe(c => {

                  this.form.get('ToCurrCode').patchValue(c['CURRCODE']);
                  this.form.get('TOCURRATE').patchValue(c['Rate'], { emitEvent: false });
                  this.form.get('TOCURROP').patchValue(c['CURROP'], { emitEvent: false });
                  this.form.get('TOCURRSK').patchValue(c['CURRSK'], { emitEvent: false });

                  let d = this.form.get('ToTransactionAmount').value;
                  this.form.get('FromTransactionAmount').patchValue(this.currencyChange(d, 'to'), { emitEvent: false });
                });
            });

          //********** Initially declare the default value and convert multi currency. */
          this._subsink.sink = this.form.get('FROMCURRATE').valueChanges
            .subscribe(data => {
              let d = this.form.get('FromTransactionAmount').value;
              this.form.get('ToTransactionAmount').patchValue(this.currencyChange(d, 'from'), { emitEvent: false });
            });

          this._subsink.sink = this.form.get('TOCURRATE').valueChanges
            .subscribe(data => {
              let d = this.form.get('ToTransactionAmount').value;
              this.form.get('FromTransactionAmount').patchValue(this.currencyChange(d, 'to'), { emitEvent: false });
            });

          this._subsink.sink = this.form.get('FromTransactionAmount').valueChanges
            .subscribe(d => {
              this.form.get('ToTransactionAmount').patchValue(this.currencyChange(d, 'from'), { emitEvent: false });
            });

          this._subsink.sink = this.form.get('ToTransactionAmount').valueChanges
            .subscribe(d => {
              this.form.get('FromTransactionAmount').patchValue(this.currencyChange(d, 'to'), { emitEvent: false });
            });
          //====================================================

          // this.form.get('FromCBSysKey').patchValue(this.form.get('FromCBSysKey').value);
          // this.form.get('ToCBSysKey').patchValue(this.form.get('ToCBSysKey').value);

          this._subsink.sink = this.findCurrency(this.form.get('ToCBSysKey').value)
            .subscribe(c => {

              this.form.get('TOCURRSK').patchValue(c['CURRSK']);
              this.form.get('ToCurrCode').patchValue(c['CURRCODE']);
            });

          this._subsink.sink = this.findCurrency(this.form.get('FromCBSysKey').value)
            .subscribe(c => {
              this.form.get('FROMCURRSK').patchValue(c['CURRSK']);
              this.form.get('FromCurrCode').patchValue(c['CURRCODE']);
            });

          let f_transaction = this.form.get('FromTransactionAmount').value;
          this.form.get('ToTransactionAmount').patchValue(this.currencyChange(f_transaction, 'from'), { emitEvent: false });

          let t_transaction = this.form.get('ToTransactionAmount').value;
          this.form.get('FromTransactionAmount').patchValue(this.currencyChange(t_transaction, 'to'), { emitEvent: false });

          this.form.get('FromCBSysKey').setValidators([Validators.required]);
          this.form.get('ToCBSysKey').setValidators([Validators.required]);
          this.form.get('TransactionDate').setValidators([Validators.required]);
          this.form.get('FromTransactionAmount').setValidators([Validators.required]);
          this.form.get('ToTransactionAmount').setValidators([Validators.required]);
        });
    });
  }

  private findCurrency(key: string) {

    return this._fromCashBookSetupService.findByKey(key)
      .pipe(map(m => {
        return m;
      }),
        mergeMap(response => {
          return this._currencyService.findByKey(response['CURRSK'])
            .pipe(map(prop => {

              response['CURRSK'] = prop['SysKey'];
              response['CURRCODE'] = prop['Currency_AlphaCode'];
              return response;
            }))
        }),
        mergeMap(response => {
          return this._currencyRateService.findByKey(response['CURRSK'])
            .pipe(map(prop => {

              response['Rate'] = prop['Rate'];
              response['CURROP'] = prop['Operator'];
              return response;
            }))
        })
      );
  }

  private currencyChange(amount: number,
    side: 'from' | 'to'): number {

    let currency = 0;

    if (side == 'from') {

      //convert to base rate
      if (this.form.get('FROMCURROP').value == "*")
        currency = amount * this.form.get('FROMCURRATE').value;
      else
        currency = amount / this.form.get('FROMCURRATE').value;

      //convert to target rate
      if (this.form.get('TOCURROP').value == "*")
        currency = currency / this.form.get('TOCURRATE').value;
      else
        currency = currency * this.form.get('TOCURRATE').value;
    }
    else if (side == 'to') {

      //convert to base rate
      if (this.form.get('TOCURRATE').value == "*")
        currency = amount * this.form.get('TOCURRATE').value;
      else
        currency = amount / this.form.get('TOCURRATE').value;

      //convert to target rate
      if (this.form.get('FROMCURROP').value == "*")
        currency = currency / this.form.get('FROMCURRATE').value;
      else
        currency = currency * this.form.get('FROMCURRATE').value;
    }

    if (isNaN(currency))
      currency = 0;

    //+ sign is to cast number datatype. 
    return +currency.toFixed(2);
  }
}
