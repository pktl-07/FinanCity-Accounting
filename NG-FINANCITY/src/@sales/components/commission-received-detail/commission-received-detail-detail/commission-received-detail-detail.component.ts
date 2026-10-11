import { Component, OnInit, OnDestroy, ViewChild, Input, AfterViewInit } from '@angular/core';
import { FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { tap, catchError, finalize } from 'rxjs/operators';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { CommissionReceivedService } from 'src/@sales/services/commission-received.service';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { ActivatedRoute } from '@angular/router';
import { SalesInvoiceService } from 'src/@sales/services/sales-invoice.service';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { paymentMode, paymentType, makeFormTouched, getComissionContext, kendoFilter, createJsonFormGroup, multiCurrency, SubSink } from 'src/@common/service/utility-service';
import { PageUtilityService } from '../page.utility.service';
import { CashbookSetupDetailComponent } from 'src/@accounts/components/cashbook-setup-detail/cashbook-setup-detail.component';
import { RecapDailogComponent } from 'src/@common/components/p-recap-dialog/p-recap-dialog.component';
import { CommissionReceivedPrintingService } from 'src/@printing/services/commission-received-printing.service';

@Component({
  selector: 'app-commission-received-detail-detail',
  templateUrl: './commission-received-detail-detail.component.html',
  providers: [
    CommissionReceivedService,
    SalesInvoiceService,
    CurrencyService,
    CurrencyRateService,
    PWindowComponentService,
    CashbookSetupService,
    CommissionReceivedPrintingService
  ]
})
export class CommissionReceivedDetailDetailComponent extends DetailFormBase implements OnDestroy, OnInit, AfterViewInit {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @Input('InvoiceSK') public invoiceSK: string;
  @ViewChild('recapDailog', { static: true }) private _recap: RecapDailogComponent;

  public _paymentMode = [];
  public _paymentType = [];
  public _subsink = new SubSink();
  public _totalBalance: number;
  public _totalGross: number;

  private _gstvalue: number = 0;
  private _grossComm: number = 0;
  private _totalAmount: number = 0;

  get readonly() {
    return this.form.get('RecordStatus').value >= 6;
  }

  constructor(fb: FormBuilder,
    private _pageUtilityService: PageUtilityService,
    public _activatedRoute: ActivatedRoute,
    public _printing: CommissionReceivedPrintingService,
    public _commissionReceivedService: CommissionReceivedService,
    public _windowService: PWindowComponentService,
    public _salesInvoiceService: SalesInvoiceService,
    public _currencyService: CurrencyService,
    public _currencyRateService: CurrencyRateService,
    public _cashbookSetupService: CashbookSetupService) {

    super(fb);

    this._commissionReceivedService.next([]);
    this._cashbookSetupService.asyncFinds();
  }

  ngOnInit(): void {

    this._paymentMode = paymentMode();
    this._paymentType = paymentType();

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._commissionReceivedService.default()
      })
      .SaveButton({
        action: () => this._commissionReceivedService.save(this.form.value).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())
        ),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._commissionReceivedService.update(this.form.value).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .PostButton({
        action: () => this._commissionReceivedService.post(this.form.value).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())),
        validate: () => {
          makeFormTouched(this.form); return this.form.valid;
        }
      })
      .ReverseButton({
        action: () => this._commissionReceivedService.reverse(this.form.value.SysKey).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())),
        validate: () => {
          makeFormTouched(this.form); return this.form.valid;
        }
      })
      .VoidButton({
        action: () => this._commissionReceivedService.void(this.form.value.SysKey).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .RecapButton({
        action: () =>
          this._recap.open(this._commissionReceivedService.recap(this.form.value.SysKey))
      })
      .ExportToPdf({
        action: () =>
          this._printing.pdf({
            filter: {
              logic: 'and',
              filters: kendoFilter([{
                field: 'SysKey', value: this.form.value.SysKey, logic: 'and', operator: 'eq'
              }])
            }
          })
      })
  }

  ngAfterViewInit(): void {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  public loadData(param: { SysKey?: string }): Promise<FormGroup> {

    return new Promise((resolve, reject) => {

      let subscribe = param.SysKey ? this._commissionReceivedService.findByKey(param.SysKey)
        : this._commissionReceivedService.default();

      this._subsink.sink = subscribe.pipe(
        catchError(error => {

          reject(error);
          return error;
        }), finalize(() => resolve(this.form)))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);

          this.form = this.fb.group(c);

          this.form.addControl('TransCURRCode', new FormControl());
          this.form.addControl('CURRCode', new FormControl());

          if (!param.SysKey || param.SysKey == null)
            this.setFullAmount('Required to set form value');
          else
            this.setFullAmount('Do not required to set form value');

          this._subsink.sink = this.form.get('PaymentType').valueChanges
            .subscribe(d => {
              if (d == 2)
                this.setFullAmount('Required to set form value');
            });

          this.setReceivableCurrency(this.form.get('CBSK').value);
          this._subsink.sink = this.form.get('CBSK').valueChanges
            .subscribe(cbsk => {

              this.setReceivableCurrency(cbsk);
            });

          this.setInvoiceCurrency();
          this.form.get('InvoiceSK').patchValue(this.invoiceSK);

          this.form.get('CBSK').setValidators([Validators.required]);
          this.form.get('ReceiptDate').setValidators([Validators.required]);
          this.form.get('AmountBeforeGST').setValidators([Validators.required, Validators.max(this._totalGross)]);
          this.form.get('TotalAmt').setValidators([Validators.required, Validators.max(this._totalBalance)]);
        })
    })
  }

  private setReceivableCurrency(cbsk: string) {

    this._subsink.sink = this._cashbookSetupService.findByKey(cbsk)
      .subscribe(cash => {

        this._subsink.sink = this._currencyService.findByKey(cash['CURRSK'])
          .subscribe(curr => {

            this.form.get('CURRSK').setValue(curr['SysKey']);
            this.form.get('CURRCode').setValue(curr['Currency_AlphaCode']);

            this._subsink.sink = this._currencyRateService.findByKey(curr['SysKey'])
              .subscribe(rate => {

                this.form.get('CURRATE').setValue(rate['Rate']);
                this.form.get('CURROP').setValue(rate['Operator']);

                this.form.get('AmountBeforeGST').patchValue(multiCurrency({
                  value: this._grossComm,
                  op: this.form.get('CURROP').value,
                  rate: this.form.get('CURRATE').value,
                }));

                this.form.get('GSTValue').patchValue(multiCurrency({
                  value: this._gstvalue,
                  op: this.form.get('CURROP').value,
                  rate: this.form.get('CURRATE').value,
                }));

                this.form.get('TotalAmt').patchValue(multiCurrency({
                  value: this._totalAmount,
                  op: this.form.get('CURROP').value,
                  rate: this.form.get('CURRATE').value,
                }));
              })
          })
      })
  }

  private async setInvoiceCurrency() {

    let inv = await this._salesInvoiceService.findByKey(this.invoiceSK).toPromise();
    let curr = await this._currencyService.findByKey(inv['SalesInvoice']['CURRSK']).toPromise();

    this.form.get('TransCURRSK').setValue(inv['SalesInvoice']['CURRSK']);
    this.form.get('TransCURRATE').setValue(inv['SalesInvoice']['CURRATE']);
    this.form.get('TransCURROP').setValue(inv['SalesInvoice']['CURROP']);
    this.form.get('TransCURRCode').setValue(curr['Currency_AlphaCode']);
  }

  private setFullAmount(required: 'Required to set form value' | 'Do not required to set form value') {

    this._commissionReceivedService.getPager({
      take: 10, skip: 0, filter: {
        filters: kendoFilter([{ field: 'InvoiceSK', value: this.invoiceSK, operator: 'eq', logic: 'and' }])
        , logic: 'and'
      }
    })

    this._subsink.sink = this._commissionReceivedService.subscribe(received => {

      if (received.data) {
        (received.data as []).forEach((d: any) => {
          let invoiceGST = d['InvoiceGST'];
          let balance = parseFloat(d['Balance']);
          let gst = parseFloat(d['InvoiceGST']);
          let gstValue = (balance - ((100 * balance)) / (gst + 100));

          this._totalGross = balance - gstValue;
          this._totalBalance = balance;

          if (required == 'Required to set form value') {

            this.form.get('GSTPercent').patchValue(invoiceGST);
            this.form.get('TotalAmt').patchValue(balance);
            this.form.get('GSTValue').patchValue(gstValue);
            this.form.get('AmountBeforeGST').patchValue(balance - gstValue);
          }
          this.cloneData();
        });
      }
    });
  }

  private _cashbookFilterText: string;

  onCashbookFilterChange(value: string) {

    this._cashbookFilterText = value;
  }

  private cloneData() {

    this._grossComm = Object.assign(this.form.get('AmountBeforeGST').value);
    this._gstvalue = Object.assign(this.form.get('GSTValue').value);
    this._totalAmount = Object.assign(this.form.get('TotalAmt').value);
  }

  addNewCashbook() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Cashbook Setup', componentType: CashbookSetupDetailComponent, state: state });
    let windowComponent = window.content.instance as CashbookSetupDetailComponent;
    windowComponent.loadData({ CashbookName: this._cashbookFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._cashbookSetupService.asyncFinds();
        this.form.get('CBSK').setValue(item.data.SysKey);
        this._windowService.close();
      });
  }

  onChangeGrossCommission() {

    let tax = this.form.get('GSTPercent').value;
    let gross = this.form.get('AmountBeforeGST').value;

    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' });

    this.form.get('GSTValue').setValue(context.taxAmount);
    this.form.get('TotalAmt').setValue(context.total);
  }

  onChangeTotalCommission() {

    let tax = this.form.get('GSTPercent').value;
    let total = this.form.get('TotalAmt').value;

    let context = getComissionContext({ tax: tax, amount: total, call: 'total' });

    this.form.get('GSTValue').setValue(context.taxAmount);
    this.form.get('AmountBeforeGST').setValue(context.gross);
  }
}
