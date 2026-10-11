import { Component, OnInit, ViewChild, OnDestroy, Inject } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { ButtonControlBuilder } from 'src/@common/components/p-toolbar/button-control.builder';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject, of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { CashbookTransactionService } from 'src/@accounts/services/cashbook-transaction.service';
import { finalize, mergeMap, map } from 'rxjs/operators';
import { makeFormTouched, createJsonFormGroup, kendoFilter, getComissionContext, cloneData, multiCurrency, SubSink } from 'src/@common/service/utility-service';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { PGridIncellEditingComponent } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.component';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { TaxService } from 'src/@accounts/services/tax.service';
import { RecapDailogComponent } from 'src/@common/components/p-recap-dialog/p-recap-dialog.component';
import { environment } from 'src/environments/environment';
import { PrintingTemplate } from './printing-template/printing-template';
import { WindowState } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-cashbook-transaction-detail',
  templateUrl: './cashbook-transaction-detail.component.html',
  providers: [
    CashbookTransactionService,
    CashbookSetupService,
    CurrencyService,
    CurrencyRateService,
    AccountSetupService,
    PWindowComponentService,
    TaxService,
  ],
  styles: [`
    .pointer-none {
      pointer-events: none;
    }
  `]
})
export class CashbookTransactionDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
  @ViewChild('cashbooktransaction', { read: PGridIncellEditingComponent, static: true }) public dataGrid: PGridIncellEditingComponent;
  @ViewChild('recapDailog', { static: true }) private _recap: RecapDailogComponent;

  private _tax = [];

  get readonly() {
    return this.form.get('RecordStatus').value >= 6;
  }

  public baseCurr = environment.general.currSK;
  private _subsink = new SubSink();
  public totalAmount = new BehaviorSubject(0);
  private gridSubject = new BehaviorSubject([]);

  constructor(
    fb: FormBuilder,
    public _cashbookService: CashbookSetupService,
    public _accountSetupService: AccountSetupService,
    public _taxService: TaxService,
    public _router: Router,
    public _windowService: PWindowComponentService,
    public _currencyService: CurrencyService,
    public _currencyRateService: CurrencyRateService,
    public _buttonControlBuilder: ButtonControlBuilder,
    public _activatedRoute: ActivatedRoute,
    public _cashbookTransactionService: CashbookTransactionService) {

    super(fb);

    this._subsink.sink = this.OnSuccess
      .subscribe(item => {

        if (item.data.RecordStatus == 0)
          this.gridSubject.next([]);
        else {

          this._subsink.sink = this._cashbookTransactionService.finds({
            filter: {
              logic: 'and',
              filters: kendoFilter([
                { field: 'ParentId', value: item.data.ParentId, logic: 'and', operator: 'eq' }
              ])
            }
          }).subscribe((data: any[]) => {

            data.map(m => {
              m['TaxAmount'] = multiCurrency({
                value: m['TaxAmount'],
                op: m['CURROP'],
                rate: m['CURRATE'],
              })

              m['TotalAmount'] = multiCurrency({
                value: m['TotalAmount'],
                op: m['CURROP'],
                rate: m['CURRATE'],
              })

              m['TransactionAmount'] = multiCurrency({
                value: m['TransactionAmount'],
                op: m['CURROP'],
                rate: m['CURRATE'],
              })
            });

            this.gridSubject.next(data);
          })
        }
      })
  }

  ngOnInit() {

    this._cashbookService.asyncFinds();

    this._subsink.sink = this.gridSubject
      .subscribe(() => {
        this.updateTotal();
      })

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {

        this._toolbar
          .controls
          .Success(this.OnSuccess)
          .Ready(this.formReady)
          .StateKey(['RecordStatus'])
          .NavigationButton({ action: () => this._router.navigateByUrl('/Cashbook/CashbookTransaction') })
          .NewButton({
            action: () =>
              this._cashbookTransactionService.default()
                .pipe(finalize(() => {
                  this.gridSubject.next([]);
                }))
          })
          .SaveButton({
            action: () =>
              this._cashbookTransactionService.save(this.bindFormValue()),
            validate: () => { makeFormTouched(this.form); return this.form.valid }
          })
          .UpdateButton({
            action: () =>
              this._cashbookTransactionService.update(this.bindFormValue()),
            validate: () => { makeFormTouched(this.form); return this.form.valid }
          })
          .PostButton({
            action: () =>
              this._cashbookTransactionService.post(this.bindFormValue()),
            validate: () => { makeFormTouched(this.form); return this.form.valid }
          })
          .ReverseButton({
            disabled: data['data']['summary']['RefTransactionType'] > 0,
            action: () =>
              this._cashbookTransactionService.reverse(this.form.get('ParentId').value),
            validate: () => { makeFormTouched(this.form); return this.form.valid }
          })
          .VoidButton({
            disabled: data['data']['summary']['RefTransactionType'] > 0,
            action: () =>
              this._cashbookTransactionService.void(this.form.get('ParentId').value),
            validate: () => { makeFormTouched(this.form); return this.form.valid }
          })
          .ExportToPdf({
            action: () => {

              let state: WindowState = 'default';
              let window = this._windowService.open({ title: 'Select salesperson to print', componentType: PrintingTemplate, state: state });
              let windowComponent = window.content.instance as PrintingTemplate;
              window.window.instance.width = 400;
              window.window.instance.height = 400;

              windowComponent.CBSK = this.form.get('SysKey').value;
              return of('')
            }
          })
          .RecapButton({
            action: () =>
              this._recap.open(this._cashbookTransactionService.recap({
                parentId: this.form.get('ParentId').value,
                type: this.form.get('TransType').value
              }))
          });

        this.loadData(data['data']['summary']);

        this._tax.push({ SysKey: null, TaxCode: 'N/A', TaxPercent: 0 });
        this._tax.push(...data['data']['tax']);

        this.dataGrid.config = {
          fetch: this.gridSubject,
          key: 'SysKey',
          columns: [
            { dataType: 'hidden', field: 'SysKey', defaultValue: null },
            { dataType: 'hidden', field: 'ParentId', defaultValue: null },
            { dataType: 'hidden', field: 'SourceHeaderTransSK', defaultValue: null },
            {
              dataType: 'dropdown', field: 'AccSK', title: 'Account', align: 'center', width: 350, validators: Validators.required, dropdown: {
                fetch: this._accountSetupService,
                textField: 'AccCode',
                valueField: 'SysKey',
                itemTemplateField: {
                  field1: 'AccCode',
                  field2: 'AccDescription'
                },
                customFilter: (e) => {
                  if (e.value.length > 2) {
                    if (e.dropDown.fetch instanceof BaseApiService) {
                      e.dropDown.fetch
                        .asyncFinds({
                          filter: {
                            filters: kendoFilter([
                              { field: 'AccCode', value: `<# ${e.value}`, operator: 'contains', logic: 'or' },
                              { field: 'AccDescription', value: `${e.value} #>`, operator: 'contains', logic: 'and' },
                              { field: 'AccountType', value: 1, operator: 'eq', logic: 'and' },
                              { field: 'IsDefault', value: false, operator: 'eq', logic: 'and' },
                            ]),
                            logic: 'or'
                          }
                        })
                    }
                  }
                }
              }
            },
            { dataType: 'string', field: 'Memo', defaultValue: null },
            {
              dataType: 'currency', field: 'TransactionAmount', width: 120, title: 'Amount', defaultValue: 0, validators: [Validators.required]
              , cellValueChange: (c) => {

                const gross = c.event;
                const inclusive = c.fg.get('Inclusive').value;
                const tax = c.fg.get('TaxPercent').value;

                if (inclusive) {
                  const taxamount = this.taxAmount({ tax: tax, amount: gross, inclusive: true });
                  c.fg.get('TaxAmount').setValue(taxamount);
                  c.fg.get('TotalAmount').setValue(gross);

                } else {
                  const taxamount = this.taxAmount({ tax: tax, amount: gross, inclusive: false });
                  c.fg.get('TaxAmount').setValue(taxamount);
                  c.fg.get('TotalAmount').setValue(taxamount + gross);
                }
              }
            },
            {
              dataType: 'dropdown', field: 'TaxSK', title: 'GST', width: 80, defaultValue: null, dropdown: {
                fetch: this._tax,
                textField: 'TaxCode',
                valueField: 'SysKey',
              }, cellValueChange: (c) => {

                const gross = c.fg.get('TransactionAmount').value;
                const tax = this._tax.find(f => f.SysKey == c.event).TaxPercent;
                const inclusive = c.fg.get('Inclusive').value;

                if (inclusive) {
                  const taxamount = this.taxAmount({ tax: tax, amount: gross, inclusive: true });
                  c.fg.get('TaxAmount').setValue(taxamount);
                  c.fg.get('TotalAmount').setValue(gross);

                } else {
                  const taxamount = this.taxAmount({ tax: tax, amount: gross, inclusive: false });
                  c.fg.get('TaxAmount').setValue(taxamount);
                  c.fg.get('TotalAmount').setValue(taxamount + gross);
                }

                c.fg.get('TaxPercent').setValue(tax);
              }
            },
            {
              dataType: 'display', field: 'TaxPercent', width: 80, title: 'GST (%)', defaultValue: 0, cellValueChange: (c) => {

                let gross = c.event;
                let tax = c.fg.get('TaxPercent').value;
                let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

                c.fg.get('TaxAmount').setValue(context.taxAmount);
                c.fg.get('TotalAmount').setValue(context.total);
              }
            },
            {
              dataType: 'chekbox', field: 'Inclusive', width: 50, title: 'Inclusive', defaultValue: false, cellValueChange: (c) => {

                const gross = +(c.fg.get('TransactionAmount').value as number).toFixed(2);
                const tax = c.fg.get('TaxPercent').value;

                if (c.event) {
                  const taxamount = this.taxAmount({ tax: tax, amount: gross, inclusive: true });
                  c.fg.get('TaxAmount').setValue(taxamount);
                  c.fg.get('TotalAmount').setValue(gross)

                } else {

                  const taxamount = this.taxAmount({ tax: tax, amount: gross, inclusive: false });
                  c.fg.get('TaxAmount').setValue(taxamount);
                  c.fg.get('TotalAmount').setValue(gross + taxamount)
                }
              }
            },
            {
              dataType: 'currency', field: 'TaxAmount', width: 120, title: 'GST ($)', defaultValue: 0, validators: [Validators.required], cellValueChange: (c => {

                const gross = c.fg.get('TransactionAmount').value;
                const inclusive = c.fg.get('Inclusive').value;

                if (inclusive) {
                  c.fg.get('TotalAmount').setValue(gross);
                } else {
                  c.fg.get('TotalAmount').setValue(c.event + gross);
                }
              })
            },
            {
              dataType: 'dispaly/currency', field: 'TotalAmount', width: 120, title: 'Total Amount', defaultValue: 0
            },
          ],
          saveCompleted: (b) => {
            this.updateTotal();
          },
          updateCompleted: (b) => {
            this.updateTotal();
          },
          deleteCompleted: (b) => {
            this.updateTotal();
          }
        };

        this.gridSubject.next(data['data']['detail']);
      });
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  private taxAmount(param: { tax: number, amount: number, inclusive: boolean }) {

    let gross = +param.amount.toFixed(2);
    let taxamount = +((gross * param.tax) / 100).toFixed(2);

    if (param.inclusive)
      return +(taxamount / ((100 + param.tax) / 100)).toFixed(2);
    else
      return taxamount;
  }

  private loadData(data: any): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve) => {

      const c: any = createJsonFormGroup(data);
      this.form = this.fb.group(c);

      this._subsink.sink = this.form.get('CBSK').valueChanges
        .subscribe(d => {

          this._subsink.sink = this._cashbookService.findByKey(d).pipe(mergeMap(response => {
            return this._currencyService.findByKey(response['CURRSK'])
              .pipe(map(a => {
                return response = a;
              }));
          }),
            mergeMap(response => {
              return this._currencyRateService.findByKey(response['SysKey'])
                .pipe(map(a => {
                  response['CURRATE'] = a['Rate'];
                  response['CURROP'] = a['Operator'];
                  return response;
                })
                )
            })
          ).subscribe((c: any) => {

            this.form.get('CURRSK').patchValue(c['SysKey']);
            this.form.get('CURRATE').patchValue(c['CURRATE']);
            this.form.get('CURROP').patchValue(c['CURROP']);

            this.form.get('CashbookCode').patchValue(c['Currency_AlphaCode']);
          })
        });

      this.form.get('TransactionDate').setValidators([Validators.required]);
      this.form.get('CBSK').setValidators([Validators.required])

      resolve(null);
    })
  }

  public bindFormValue(): any {

    const insertedItems = this.bindMap(this.dataGrid.service.data.filter(x => x['SysKey'] == null));
    const updatedItems = this.bindMap(this.dataGrid.service.data.filter(x => x['SysKey'] != null));
    const deletedItems = this.bindMap(this.dataGrid.service.deletedItems);

    return {
      insertedItems: insertedItems,
      updatedItems: updatedItems,
      deletedItems: deletedItems,
    }
  }

  private bindMap(data: any[]) {

    data.map(m => {
      m['R1'] = this.form.get('R1').value;
      m['Prefix'] = this.form.get('Prefix').value;
      m['Postfix'] = this.form.get('Postfix').value;
      m['VoucherId'] = this.form.get('VoucherId').value;

      m['RecordStatus'] = this.form.get('RecordStatus').value;
      m['ManualRef'] = this.form.get('ManualRef').value;
      m['CBSK'] = this.form.get('CBSK').value;
      m['TransType'] = this.form.get('TransType').value;
      m['TransactionDate'] = this.form.get('TransactionDate').value;
      m['RefTransactionType'] = this.form.get('RefTransactionType').value;

      m['PaidPerson'] = this.form.get('PaidPerson').value;
      m['BankInfo'] = this.form.get('BankInfo').value;

      m['ChequeNo'] = this.form.get('ChequeNo').value;
      m['SecondRef'] = this.form.get('SecondRef').value;
      m['Description'] = this.form.get('Description').value;
      m['JRef'] = this.form.get('JRef').value;

      m['CURRSK'] = this.form.get('CURRSK').value;
      m['CURRATE'] = this.form.get('CURRATE').value;
      m['CURROP'] = this.form.get('CURROP').value;
    })
    return data;
  }

  //refresh total debit and credit amount.
  private updateTotal() {
    let total = cloneData(this.gridSubject.value as any[]);
    this.totalAmount.next(total.reduce((sum, current) => sum + current['TotalAmount'], 0));
  }
}
